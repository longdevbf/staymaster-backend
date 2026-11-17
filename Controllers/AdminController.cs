using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.DTOs.Booking;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using HotelManagement.API.Services.Email;

namespace HotelManagement.API.Controllers;

public class UpdateSettingsRequest
{
    /// <summary>Phần trăm, ví dụ 10 = 10%</summary>
    public decimal ServiceFeePercent { get; set; }
    public decimal TaxPercent { get; set; }
    public int PaymentHoldMinutes { get; set; }
    public string SupportEmail { get; set; } = string.Empty;
    public string SupportPhone { get; set; } = string.Empty;
}

[ApiController]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ISystemSettings _settings;
    private readonly IBookingService _bookings;
    private readonly IAuditService _audit;
    private readonly EmailOptions _email;

    public AdminController(AppDbContext db, ISystemSettings settings, IBookingService bookings, IAuditService audit,
        IOptions<EmailOptions> email)
    {
        _db = db;
        _settings = settings;
        _bookings = bookings;
        _audit = audit;
        _email = email.Value;
    }

    // GET /api/settings/public — phí, thuế, thời gian giữ phòng, liên hệ hỗ trợ (ai cũng xem được)
    [HttpGet("api/settings/public")]
    [AllowAnonymous]
    public async Task<IActionResult> PublicSettings()
    {
        var s = await _settings.GetAsync();
        // EmailEnabled: có SMTP thì mới gửi email; không có thì mọi thông báo chỉ hiện trong chuông thông báo
        return Ok(new { s.ServiceFeeRate, s.TaxRate, s.PaymentHoldMinutes, s.SupportEmail, s.SupportPhone, EmailEnabled = _email.IsConfigured });
    }

    // GET /api/admin/settings
    [HttpGet("api/admin/settings")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetSettings()
    {
        var s = await _settings.GetAsync();
        return Ok(new
        {
            ServiceFeePercent = s.ServiceFeeRate * 100,
            TaxPercent = s.TaxRate * 100,
            s.PaymentHoldMinutes,
            s.SupportEmail,
            s.SupportPhone,
        });
    }

    // PUT /api/admin/settings
    [HttpPut("api/admin/settings")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingsRequest req)
    {
        var before = await _settings.GetAsync();
        var next = new SystemSettingsSnapshot(
            Math.Round(req.ServiceFeePercent / 100, 4),
            Math.Round(req.TaxPercent / 100, 4),
            req.PaymentHoldMinutes,
            req.SupportEmail?.Trim() ?? "",
            req.SupportPhone?.Trim() ?? "");
        var error = await _settings.UpdateAsync(next, User.GetUserId());
        if (error is not null) return BadRequest(new { message = error });
        await _audit.LogAsync("SETTINGS_UPDATE", "cau_hinh_he_thong", null, before, next);
        return await GetSettings();
    }

    // GET /api/admin/todo — việc đang chờ admin xử lý
    [HttpGet("api/admin/todo")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Todo()
    {
        var since = DateTime.Now.AddHours(-24);
        var pendingRooms = await _db.Rooms.CountAsync(r => r.Status == "pending");
        var pendingPayouts = await _db.Payouts.Where(p => p.Status == "pending")
            .GroupBy(_ => 1).Select(g => new { Count = g.Count(), Amount = g.Sum(p => p.Amount) }).FirstOrDefaultAsync();
        var waitingSupport = await _db.InboxQuestions.CountAsync(q => q.PartnerId == null && q.Status == "open");
        var failedLogins = await _db.AuditLogs.Where(l => l.Action == "LOGIN_FAILED" && l.CreatedAt >= since)
            .GroupBy(l => l.EntityKey)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .Where(g => g.Count >= 5)
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToListAsync();
        var refunds = await _db.AuditLogs.CountAsync(l => l.Action == "PAYMENT_NEEDS_REFUND" && l.CreatedAt >= DateTime.Now.AddDays(-30));
        var hiddenReviews = await _db.Reviews.CountAsync(r => r.Stars <= 2 && !r.IsHidden && r.CreatedAt >= DateTime.Now.AddDays(-7));

        return Ok(new
        {
            PendingRooms = pendingRooms,
            PendingPayouts = pendingPayouts?.Count ?? 0,
            PendingPayoutAmount = pendingPayouts?.Amount ?? 0,
            WaitingSupport = waitingSupport,
            SuspiciousLogins = failedLogins.Select(f => new { Account = f.Key, Attempts = f.Count }),
            RefundsToCheck = refunds,
            LowReviewsThisWeek = hiddenReviews,
        });
    }

    // GET /api/admin/bookings?status=&from=&to=&propertyId=&search=&page=&pageSize=
    [HttpGet("api/admin/bookings")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Bookings([FromQuery] string? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] int? propertyId, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var all = await _bookings.GetAllBookingsAsync();
        IEnumerable<BookingResponse> q = all;
        if (!string.IsNullOrWhiteSpace(status) && status != "all") q = q.Where(b => b.Status == status);
        if (from is not null) q = q.Where(b => b.CheckOut.Date > from.Value.Date);
        if (to is not null) q = q.Where(b => b.CheckIn.Date <= to.Value.Date);
        if (propertyId is not null) q = q.Where(b => b.PropertyId == propertyId);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            var id = s.TrimStart('#', 'B', 'K', 'b', 'k');
            q = q.Where(b => b.Id.ToString() == id ||
                             b.UserName.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                             (b.UserEmail ?? "").Contains(s, StringComparison.OrdinalIgnoreCase) ||
                             (b.UserPhone ?? "").Contains(s) ||
                             b.RoomName.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                             (b.PropertyName ?? "").Contains(s, StringComparison.OrdinalIgnoreCase));
        }
        var list = q.ToList();
        var counts = all.GroupBy(b => b.Status ?? "pending").ToDictionary(g => g.Key, g => g.Count());
        return Ok(new
        {
            Total = list.Count,
            Page = page,
            PageSize = pageSize,
            Counts = counts,
            TotalValue = list.Where(b => b.Status != BookingStatus.Cancelled).Sum(b => b.TotalPrice),
            Items = list.Skip((page - 1) * pageSize).Take(pageSize),
        });
    }

    // GET /api/admin/properties — danh sách cơ sở để lọc
    [HttpGet("api/admin/properties")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Properties() =>
        Ok(await _db.Properties.OrderBy(p => p.Name).Select(p => new { p.Id, p.Name, p.City }).ToListAsync());
}
