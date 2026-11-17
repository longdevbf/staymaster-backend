using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.DTOs.Booking;
using HotelManagement.API.Models;
using HotelManagement.API.Services.Email;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Services;

/// <summary>Who may act on a booking: the customer who made it and the partner who owns the room.</summary>
public record BookingAccessInfo(int BookingId, int CustomerUserId, int? PartnerUserId, string Status, decimal AmountDue);

public interface IBookingService
{
    Task<(BookingResponse? booking, string? error)> CreateBookingAsync(int userId, CreateBookingRequest req);
    Task<List<BookingResponse>> GetUserBookingsAsync(int userId);
    Task<List<BookingResponse>> GetAllBookingsAsync();
    Task<BookingResponse?> GetBookingByIdAsync(int bookingId);
    Task<BookingAccessInfo?> GetAccessInfoAsync(int bookingId);
    Task<BookingResponse?> ConfirmBookingAsync(int bookingId);
    Task<(BookingResponse? booking, string? error)> CheckInAsync(int bookingId);
    Task<(BookingResponse? booking, string? error)> CompleteAsync(int bookingId);
    Task<BookingResponse?> CancelBookingAsync(int bookingId, string? reason = null);
    Task<BookingResponse?> SyncSePayPaymentAsync(int bookingId);
    Task<List<BookingResponse>> GetPartnerBookingsAsync(int partnerId);
    Task<List<BookingResponse>> GetPartnerBookingsByUserIdAsync(int userId);
    Task<List<OccupancyRateResponse>> GetOccupancyRatesAsync(DateTime from, DateTime to);
    /// <summary>Huỷ các đơn chưa thanh toán đã quá thời gian giữ phòng. Trả về số đơn đã huỷ.</summary>
    Task<int> ExpireUnpaidAsync(CancellationToken ct = default);
    /// <summary>Tiền về cho đơn không còn chờ thanh toán (ví dụ đã tự huỷ vì quá hạn).</summary>
    Task<string> HandlePaymentForInactiveBookingAsync(int bookingId, decimal amount);
    Task<(PriceQuote? quote, string? error)> QuoteAsync(int roomId, DateTime checkIn, DateTime checkOut);
    /// <summary>Các khoảng ngày phòng đã có người đặt hoặc bị chặn (để làm mờ trên lịch)</summary>
    Task<List<BookedRange>> GetBookedRangesAsync(int roomId);
}

public record BookedRange(DateTime From, DateTime To, string Kind);

public class BookingService : IBookingService
{
    private readonly AppDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<BookingService> _logger;
    private readonly INotificationService _notifications;
    private readonly IAuditService _audit;
    private readonly ISystemSettings _settings;
    private const decimal DEPOSIT_RATE = 0.30m;
    public const string ExpireAction = "BOOKING_EXPIRE";

    public BookingService(AppDbContext db, IHttpClientFactory httpClientFactory, IConfiguration config,
        ILogger<BookingService> logger, INotificationService notifications, IAuditService audit, ISystemSettings settings)
    {
        _audit = audit;
        _db = db;
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
        _notifications = notifications;
        _settings = settings;
    }

    /// <summary>
    /// Đơn đang chiếm phòng trong khoảng ngày: mọi đơn chưa huỷ, trừ đơn chờ thanh toán đã quá hạn giữ phòng
    /// (phòng phải mở lại ngay cả khi job tự huỷ chưa kịp chạy).
    /// </summary>
    private IQueryable<Booking> Blocking(IQueryable<Booking> q, DateTime holdCutoff) =>
        q.Where(b => b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.CancelledVi &&
                     !(b.Status == BookingStatus.Pending && b.CreatedAt < holdCutoff));

    public async Task<(PriceQuote? quote, string? error)> QuoteAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        if (checkIn.Date >= checkOut.Date) return (null, "Ngày nhận phòng phải trước ngày trả phòng");
        if ((checkOut.Date - checkIn.Date).TotalDays > 60) return (null, "Mỗi đơn đặt tối đa 60 đêm");
        var room = await _db.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roomId);
        if (room is null) return (null, "Phòng không tồn tại");
        return (Pricing.Quote(room, checkIn, checkOut, await _settings.GetAsync()), null);
    }

    public async Task<List<BookedRange>> GetBookedRangesAsync(int roomId)
    {
        var settings = await _settings.GetAsync();
        var cutoff = DateTime.Now.AddMinutes(-settings.PaymentHoldMinutes);
        var today = AppTime.Today;
        var bookings = await Blocking(_db.Bookings.Where(b => b.RoomId == roomId && b.CheckOut > today), cutoff)
            .OrderBy(b => b.CheckIn)
            .Select(b => new BookedRange(b.CheckIn.Date, b.CheckOut.Date, "booked"))
            .ToListAsync();
        var blocks = await _db.RoomBlocks.Where(b => b.RoomId == roomId && b.ToDate > today)
            .Select(b => new BookedRange(b.FromDate, b.ToDate, "blocked"))
            .ToListAsync();
        return bookings.Concat(blocks).OrderBy(r => r.From).ToList();
    }

    public async Task<(BookingResponse? booking, string? error)> CreateBookingAsync(
        int userId, CreateBookingRequest req)
    {
        // Validate ngày
        if (req.CheckIn.Date >= req.CheckOut.Date)
            return (null, "Ngày check-in phải trước ngày check-out");

        if (req.CheckIn.Date < AppTime.Today)
            return (null, "Ngày check-in không được trong quá khứ");

        if ((req.CheckOut.Date - req.CheckIn.Date).TotalDays > 60)
            return (null, "Mỗi đơn đặt tối đa 60 đêm");

        // Lấy phòng
        var room = await _db.Rooms.FindAsync(req.RoomId);
        if (room is null) return (null, "Phòng không tồn tại");
        if (room.IsApproved != true || room.Status is "hidden" or "rejected" or "deleted")
            return (null, "Phòng hiện không nhận đặt");
        if (room.Status == "maintenance")
            return (null, "Phòng đang bảo trì");
        if (req.GuestCount < 1 || req.GuestCount > room.Capacity)
            return (null, $"Phòng chỉ nhận tối đa {room.Capacity} khách");

        var settings = await _settings.GetAsync();
        var holdCutoff = DateTime.Now.AddMinutes(-settings.PaymentHoldMinutes);

        // Thuật toán 4.2: Kiểm tra trùng lịch
        bool isOverlapping = await Blocking(_db.Bookings, holdCutoff).AnyAsync(b =>
            b.RoomId == req.RoomId &&
            req.CheckIn < b.CheckOut &&
            req.CheckOut > b.CheckIn
        );
        if (isOverlapping)
            return (null, "Phòng đã được đặt trong khoảng thời gian này");

        bool isBlocked = await _db.RoomBlocks.AnyAsync(b =>
            b.RoomId == req.RoomId && req.CheckIn < b.ToDate && req.CheckOut > b.FromDate);
        if (isBlocked)
            return (null, "Chủ nhà tạm ngưng nhận khách trong khoảng ngày này");

        // Thuật toán 4.3: Tính tổng tiền (tiền phòng có giá cuối tuần + phí dịch vụ + thuế)
        var quote = Pricing.Quote(room, req.CheckIn, req.CheckOut, settings);

        // Thuật toán 4.4: Tính tiền cọc
        decimal? deposit = req.IsDeposit
            ? Math.Round(quote.Total * DEPOSIT_RATE, 2)
            : quote.Total;

        var booking = new Booking
        {
            UserId = userId,
            RoomId = req.RoomId,
            CheckIn = req.CheckIn.Date,
            CheckOut = req.CheckOut.Date,
            Nights = quote.Nights,
            GuestCount = req.GuestCount,
            PricePerNight = room.BasePrice,
            ServiceFee = quote.ServiceFee,
            Tax = quote.Tax,
            TotalPrice = quote.Total,
            Deposit = deposit,
            Note = string.IsNullOrWhiteSpace(req.Note) ? null : req.Note.Trim(),
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.Now
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Booking {BookingId} created by user {UserId} for room {RoomId}", booking.Id, userId, req.RoomId);
        await _notifications.NotifyBookingAsync(booking.Id, BookingEvent.Created);
        await _audit.LogAsync("BOOKING_CREATE", "dat_phong", booking.Id, null,
            new { booking.RoomId, booking.CheckIn, booking.CheckOut, booking.TotalPrice }, userId);
        return (await GetBookingByIdAsync(booking.Id), null);
    }

    public async Task<List<BookingResponse>> GetUserBookingsAsync(int userId) =>
        await MapManyAsync(_db.Bookings.Where(b => b.UserId == userId));

    public async Task<List<BookingResponse>> GetAllBookingsAsync() =>
        await MapManyAsync(_db.Bookings);

    public async Task<BookingResponse?> GetBookingByIdAsync(int bookingId)
    {
        var list = await MapManyAsync(_db.Bookings.Where(b => b.Id == bookingId));
        return list.FirstOrDefault();
    }

    public async Task<BookingAccessInfo?> GetAccessInfoAsync(int bookingId)
    {
        return await _db.Bookings
            .Where(b => b.Id == bookingId)
            .Select(b => new BookingAccessInfo(
                b.Id,
                b.UserId,
                (int?)b.Room.Property.Partner.UserId,
                b.Status ?? BookingStatus.Pending,
                b.Deposit ?? b.TotalPrice))
            .FirstOrDefaultAsync();
    }

    public async Task<BookingResponse?> ConfirmBookingAsync(int bookingId)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking is null || !BookingStatus.Is(booking.Status, BookingStatus.Pending)) return null;

        booking.Status = BookingStatus.Confirmed;
        booking.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("BOOKING_CONFIRM", "dat_phong", bookingId, new { status = BookingStatus.Pending }, new { status = booking.Status });

        _logger.LogInformation("Booking {BookingId} confirmed", bookingId);
        await _notifications.NotifyBookingAsync(bookingId, BookingEvent.Confirmed);
        return await GetBookingByIdAsync(bookingId);
    }

    public async Task<(BookingResponse? booking, string? error)> CheckInAsync(int bookingId)
    {
        var booking = await _db.Bookings.Include(b => b.Room).FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking is null) return (null, "Không tìm thấy đơn");
        if (!BookingStatus.Is(booking.Status, BookingStatus.Confirmed))
            return (null, BookingStatus.Is(booking.Status, BookingStatus.Pending)
                ? "Đơn chưa thanh toán nên chưa thể nhận phòng"
                : "Chỉ nhận phòng cho đơn đã xác nhận");

        // Chỉ nhận phòng từ ngày nhận phòng tới trước ngày trả phòng
        if (AppTime.Today < booking.CheckIn.Date)
            return (null, $"Chưa đến ngày nhận phòng ({booking.CheckIn:dd/MM/yyyy})");
        if (AppTime.Today >= booking.CheckOut.Date)
            return (null, $"Đã qua ngày trả phòng ({booking.CheckOut:dd/MM/yyyy}); liên hệ quản trị viên nếu khách vẫn đến");

        booking.Status = BookingStatus.CheckedIn;
        booking.UpdatedAt = DateTime.Now;
        booking.CheckInTime = DateTime.Now.TimeOfDay;
        booking.Room.Status = "occupied";
        await _db.SaveChangesAsync();
        await _audit.LogAsync("BOOKING_CHECKIN", "dat_phong", bookingId, new { status = BookingStatus.Confirmed }, new { status = booking.Status });

        _logger.LogInformation("Booking {BookingId} checked in", bookingId);
        return (await GetBookingByIdAsync(bookingId), null);
    }

    public async Task<(BookingResponse? booking, string? error)> CompleteAsync(int bookingId)
    {
        var booking = await _db.Bookings
            .Include(b => b.Room).ThenInclude(r => r.Property).ThenInclude(p => p.Partner)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking is null) return (null, "Không tìm thấy đơn");
        if (!BookingStatus.Is(booking.Status, BookingStatus.CheckedIn))
            return (null, "Chỉ trả phòng cho khách đang lưu trú");

        booking.Status = BookingStatus.Completed;
        booking.UpdatedAt = DateTime.Now;
        booking.CheckOutTime = DateTime.Now.TimeOfDay;
        booking.Room.Status = "available";

        // Doanh thu cộng cho Đối tác khi chuyến đi hoàn tất (SRS 2d): khách trả trừ phí dịch vụ của StayMaster
        var earning = Pricing.PartnerEarning(booking);
        var partner = booking.Room.Property.Partner;
        if (partner != null)
        {
            partner.WalletBalance += earning;
        }

        await _db.SaveChangesAsync();
        await _audit.LogAsync("BOOKING_COMPLETE", "dat_phong", bookingId, new { status = BookingStatus.CheckedIn },
            new { status = booking.Status, partnerCredited = earning, platformFee = booking.ServiceFee });

        _logger.LogInformation("Booking {BookingId} completed; partner credited {Amount}", bookingId, earning);
        await _notifications.NotifyBookingAsync(bookingId, BookingEvent.Completed);
        return (await GetBookingByIdAsync(bookingId), null);
    }

    public async Task<BookingResponse?> CancelBookingAsync(int bookingId, string? reason = null)
    {
        var booking = await _db.Bookings.Include(b => b.Room).FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking is null) return null;

        // Chỉ huỷ được đơn chưa nhận phòng
        var status = BookingStatus.Normalize(booking.Status);
        if (status != BookingStatus.Pending && status != BookingStatus.Confirmed) return null;

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.Now;
        if (booking.Room.Status == "occupied")
            booking.Room.Status = "available";

        await _db.SaveChangesAsync();
        await _audit.LogAsync("BOOKING_CANCEL", "dat_phong", bookingId, new { status }, new { status = booking.Status, reason });

        _logger.LogInformation("Booking {BookingId} cancelled", bookingId);
        await _notifications.NotifyBookingAsync(bookingId, BookingEvent.Cancelled, reason);
        return await GetBookingByIdAsync(bookingId);
    }

    public async Task<int> ExpireUnpaidAsync(CancellationToken ct = default)
    {
        var settings = await _settings.GetAsync();
        var cutoff = DateTime.Now.AddMinutes(-settings.PaymentHoldMinutes);
        var expired = await _db.Bookings
            .Where(b => b.Status == BookingStatus.Pending && b.CreatedAt < cutoff)
            .OrderBy(b => b.Id)
            .Take(200)
            .ToListAsync(ct);
        if (expired.Count == 0) return 0;

        foreach (var b in expired)
        {
            b.Status = BookingStatus.Cancelled;
            b.UpdatedAt = DateTime.Now;
        }
        await _db.SaveChangesAsync(ct);

        foreach (var b in expired)
        {
            await _audit.LogAsync(ExpireAction, "dat_phong", b.Id, new { status = BookingStatus.Pending },
                new { status = BookingStatus.Cancelled, holdMinutes = settings.PaymentHoldMinutes }, userId: b.UserId);
            await _notifications.NotifyBookingAsync(b.Id, BookingEvent.Expired);
        }
        _logger.LogInformation("Expired {Count} unpaid bookings older than {Minutes} minutes", expired.Count, settings.PaymentHoldMinutes);
        return expired.Count;
    }

    public async Task<string> HandlePaymentForInactiveBookingAsync(int bookingId, decimal amount)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking is null) return "Booking not found";

        var status = BookingStatus.Normalize(booking.Status);
        var wasExpired = status == BookingStatus.Cancelled &&
            await _db.AuditLogs.AnyAsync(l => l.Action == ExpireAction && l.EntityKey == bookingId.ToString());

        if (wasExpired && amount >= (booking.Deposit ?? booking.TotalPrice) && booking.CheckIn.Date >= AppTime.Today)
        {
            // Khách chuyển khoản trễ: nếu phòng vẫn trống thì khôi phục và xác nhận đơn
            var settings = await _settings.GetAsync();
            var cutoff = DateTime.Now.AddMinutes(-settings.PaymentHoldMinutes);
            var taken = await Blocking(_db.Bookings, cutoff).AnyAsync(b =>
                b.Id != bookingId && b.RoomId == booking.RoomId && booking.CheckIn < b.CheckOut && booking.CheckOut > b.CheckIn);
            var blocked = await _db.RoomBlocks.AnyAsync(b =>
                b.RoomId == booking.RoomId && booking.CheckIn < b.ToDate && booking.CheckOut > b.FromDate);
            if (!taken && !blocked)
            {
                booking.Status = BookingStatus.Confirmed;
                booking.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
                await _audit.LogAsync("BOOKING_REINSTATE", "dat_phong", bookingId, new { status }, new { status = booking.Status, amount });
                await _notifications.NotifyBookingAsync(bookingId, BookingEvent.Confirmed);
                return "Late payment: booking reinstated and confirmed";
            }
        }

        // Không khôi phục được: báo admin để hoàn tiền thủ công
        await _audit.LogAsync("PAYMENT_NEEDS_REFUND", "dat_phong", bookingId, null, new { status, amount });
        await _notifications.NotifyPaymentNeedsRefundAsync(bookingId, amount);
        return "Payment recorded for an inactive booking; admin notified for refund";
    }

    /// <summary>
    /// Hỏi SePay xem đã có giao dịch chuyển khoản "BK{id}" đủ số tiền chưa; có thì xác nhận đơn.
    /// </summary>
    public async Task<BookingResponse?> SyncSePayPaymentAsync(int bookingId)
    {
        var booking = await _db.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking == null) return null;
        if (!BookingStatus.Is(booking.Status, BookingStatus.Pending)) return await GetBookingByIdAsync(bookingId);

        var apiKey = _config["SePay:ApiKey"];
        if (string.IsNullOrEmpty(apiKey)) return await GetBookingByIdAsync(bookingId);

        var amountDue = booking.Deposit ?? booking.TotalPrice;

        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        try
        {
            var url = $"https://userapi.sepay.vn/v2/transactions?page=1&per_page=20&transaction_date_sort=desc&q=BK{bookingId}";
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return await GetBookingByIdAsync(bookingId);

            var json = await response.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(json);

            // SePay v2 response: { "status": "success", "data": [...] }
            if (doc.RootElement.TryGetProperty("data", out var transactions))
            {
                foreach (var tx in transactions.EnumerateArray())
                {
                    var content = tx.TryGetProperty("transaction_content", out var contentEl) ? contentEl.GetString() ?? "" : "";
                    var amountIn = tx.TryGetProperty("amount_in", out var amountEl) ? amountEl.GetDecimal() : 0;

                    if (MatchesBooking(content, bookingId) && amountIn >= amountDue)
                    {
                        _logger.LogInformation("SePay sync: payment {Amount} found for booking {BookingId}", amountIn, bookingId);
                        return await ConfirmBookingAsync(bookingId);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SePay sync failed for booking {BookingId}", bookingId);
        }

        return await GetBookingByIdAsync(bookingId);
    }

    /// <summary>Nội dung chuyển khoản có đúng mã "BK{id}" (không nhầm BK12 với BK123).</summary>
    public static bool MatchesBooking(string content, int bookingId) =>
        System.Text.RegularExpressions.Regex.IsMatch(content, $@"BK{bookingId}(?!\d)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    public async Task<List<OccupancyRateResponse>> GetOccupancyRatesAsync(DateTime from, DateTime to)
    {
        int totalDays = (int)(to - from).TotalDays;
        if (totalDays <= 0) return new();

        var rooms = await _db.Rooms.Include(r => r.Bookings).ToListAsync();

        return rooms.Select(room =>
        {
            int occupiedDays = room.Bookings
                .Where(b => !BookingStatus.Is(b.Status, BookingStatus.Cancelled))
                .Sum(b =>
                {
                    var start = b.CheckIn > from ? b.CheckIn : from;
                    var end = b.CheckOut < to ? b.CheckOut : to;
                    return start < end ? (int)(end - start).TotalDays : 0;
                });

            return new OccupancyRateResponse
            {
                RoomId = room.Id,
                RoomCode = room.RoomCode,
                RoomName = room.Name,
                OccupancyRate = Math.Round((double)occupiedDays / totalDays * 100, 2),
                TotalDays = totalDays,
                OccupiedDays = occupiedDays
            };
        }).ToList();
    }

    public async Task<List<BookingResponse>> GetPartnerBookingsAsync(int partnerId) =>
        await MapManyAsync(_db.Bookings.Where(b => b.Room.Property.PartnerId == partnerId));

    public async Task<List<BookingResponse>> GetPartnerBookingsByUserIdAsync(int userId)
    {
        var partner = await _db.Partners.FirstOrDefaultAsync(p => p.UserId == userId);
        if (partner == null) return new List<BookingResponse>();

        return await GetPartnerBookingsAsync(partner.Id);
    }

    /// <summary>Đọc đơn kèm phòng, cơ sở, khách và đánh giá (nếu có) trong một truy vấn.</summary>
    private async Task<List<BookingResponse>> MapManyAsync(IQueryable<Booking> query)
    {
        var settings = await _settings.GetAsync();
        var rows = await query
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new
            {
                Booking = b,
                RoomName = b.Room.Name,
                b.Room.RoomCode,
                RoomImage = b.Room.Images.OrderByDescending(i => i.IsMainImage).ThenBy(i => i.DisplayOrder).Select(i => i.Url).FirstOrDefault(),
                UserName = b.User.FullName,
                UserAvatar = b.User.Avatar,
                UserPhone = b.User.Phone,
                UserEmail = b.User.Email,
                PropertyId = b.Room.PropertyId,
                PropertyName = b.Room.Property.Name,
                PropertyAddress = b.Room.Property.DetailedAddress,
                b.Room.Property.Ward,
                b.Room.Property.District,
                b.Room.Property.City,
                b.Room.Property.Latitude,
                b.Room.Property.Longitude,
                b.Room.Property.DefaultCheckInTime,
                b.Room.Property.DefaultCheckOutTime,
                HostName = b.Room.Property.Partner.BusinessName,
                HostPhone = b.Room.Property.Partner.User != null ? b.Room.Property.Partner.User.Phone : null,
                Review = _db.Reviews.Where(r => r.BookingId == b.Id).Select(r => new { r.Id, r.Stars }).FirstOrDefault(),
            })
            .AsNoTracking()
            .ToListAsync();

        return rows.Select(x =>
        {
            var b = x.Booking;
            var status = BookingStatus.Normalize(b.Status);
            var address = string.Join(", ", new[] { x.PropertyAddress, x.Ward, x.District, x.City }
                .Where(p => !string.IsNullOrWhiteSpace(p) && p != "Chưa cập nhật"));
            return new BookingResponse
            {
                Id = b.Id,
                RoomId = b.RoomId,
                RoomName = x.RoomName,
                RoomCode = x.RoomCode,
                UserId = b.UserId,
                UserName = x.UserName,
                UserPhone = x.UserPhone,
                UserEmail = x.UserEmail,
                CheckIn = b.CheckIn,
                CheckOut = b.CheckOut,
                Nights = b.Nights,
                GuestCount = b.GuestCount,
                PricePerNight = b.PricePerNight,
                CleaningFee = b.CleaningFee,
                ServiceFee = b.ServiceFee,
                Tax = b.Tax,
                TotalPrice = b.TotalPrice,
                Deposit = b.Deposit,
                PartnerEarning = Pricing.PartnerEarning(b),
                Status = status,
                Note = b.Note,
                RoomImage = x.RoomImage,
                UserAvatar = x.UserAvatar,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                PaymentDeadline = status == BookingStatus.Pending && b.CreatedAt is not null
                    ? b.CreatedAt.Value.AddMinutes(settings.PaymentHoldMinutes)
                    : null,
                PropertyId = x.PropertyId,
                PropertyName = x.PropertyName,
                PropertyAddress = address,
                Latitude = x.Latitude is null or 0 ? null : (double)x.Latitude,
                Longitude = x.Longitude is null or 0 ? null : (double)x.Longitude,
                CheckInTime = (x.DefaultCheckInTime ?? new TimeSpan(14, 0, 0)).ToString(@"hh\:mm"),
                CheckOutTime = (x.DefaultCheckOutTime ?? new TimeSpan(12, 0, 0)).ToString(@"hh\:mm"),
                HostName = x.HostName,
                HostPhone = status is BookingStatus.Confirmed or BookingStatus.CheckedIn ? x.HostPhone : null,
                ReviewId = x.Review?.Id,
                ReviewStars = x.Review?.Stars,
            };
        }).ToList();
    }
}
