using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.DTOs.Statistics;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatisticsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IRecommendationService _recommendations;

    public StatisticsController(AppDbContext context, IRecommendationService recommendations)
    {
        _context = context;
        _recommendations = recommendations;
    }

    /// <summary>
    /// API 1.1: Tính tổng doanh thu (revenue) của toàn bộ hệ thống
    /// </summary>
    [HttpGet("revenue")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<RevenueResponse>> GetTotalSystemRevenue()
    {
        var total = await _context.Bookings
            .Where(b => b.Status != "Hủy" && b.Status != "Cancelled")
            .SumAsync(b => (decimal?)b.TotalPrice) ?? 0;

        return Ok(new RevenueResponse { TotalRevenue = total });
    }

    /// <summary>
    /// API 1.2: Tính tổng doanh thu (revenue) của riêng 1 Doanh Nghiệp / Cơ sở
    /// </summary>
    [HttpGet("revenue/property/{propertyId}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<RevenueResponse>> GetPropertyRevenue(int propertyId)
    {
        var total = await _context.Bookings
            .Where(b => b.Status != "Hủy" && b.Status != "Cancelled" && b.Room.PropertyId == propertyId)
            .SumAsync(b => (decimal?)b.TotalPrice) ?? 0;

        return Ok(new RevenueResponse { TotalRevenue = total });
    }

    /// <summary>
    /// API 2: Tính tỷ lệ phòng được thuê trong hôm nay (occupancy rate)
    /// </summary>
    [HttpGet("occupancy-rate")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<OccupancyRateResponse>> GetOccupancyRate()
    {
        var totalRooms = await _context.Rooms.CountAsync();
        
        // Đếm các phòng đang có Booking bao trùm ngày hôm nay
        var today = AppTime.Today;
        var bookedRoomsCount = await _context.Bookings
            .Where(b => b.CheckIn <= today && b.CheckOut >= today && b.Status != "Hủy" && b.Status != "Cancelled")
            .Select(b => b.RoomId)
            .Distinct()
            .CountAsync();

        double percentage = totalRooms > 0 ? ((double)bookedRoomsCount / totalRooms) * 100 : 0;

        return Ok(new OccupancyRateResponse
        {
            Percentage = Math.Round(percentage, 2),
            TotalRooms = totalRooms,
            BookedRooms = bookedRoomsCount
        });
    }

    /// <summary>
    /// API 3: Lấy danh sách Top 5 phòng được thuê nhiều nhất
    /// </summary>
    [HttpGet("top-booked-rooms")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<IEnumerable<TopBookedRoomDto>>> GetTopBookedRooms()
    {
        var topRooms = await _context.Bookings
            .Where(b => b.Status != "Hủy" && b.Status != "Cancelled")
            .GroupBy(b => b.RoomId)
            .Select(g => new
            {
                RoomId = g.Key,
                BookingCount = g.Count()
            })
            .OrderByDescending(x => x.BookingCount)
            .Take(5)
            .Join(_context.Rooms,
                  b => b.RoomId,
                  r => r.Id,
                  (b, r) => new TopBookedRoomDto
                  {
                      RoomId = r.Id,
                      RoomName = r.Name,
                      BookingCount = b.BookingCount
                  })
            .ToListAsync();

        return Ok(topRooms);
    }

    /// <summary>
    /// API 4: Đề xuất tối đa 8 phòng theo vị trí hiện tại, điểm đánh giá, lượt đặt và lịch sử lưu trú.
    /// Không bắt buộc đăng nhập; nếu có token thì dùng thêm lịch sử của người dùng.
    /// </summary>
    [HttpGet("recommendations")]
    public async Task<ActionResult<IEnumerable<RoomRecommendationDto>>> GetRoomRecommendations(
        [FromQuery] double? lat, [FromQuery] double? lng, [FromQuery] int limit = 8)
    {
        if (lat is < -90 or > 90 || lng is < -180 or > 180)
            return BadRequest(new { message = "Toạ độ không hợp lệ" });

        return Ok(await _recommendations.RecommendAsync(User.GetUserId(), lat, lng, limit));
    }

    /// <summary>
    /// Báo cáo doanh thu toàn hệ thống: theo tháng (12 tháng gần nhất) và theo cơ sở.
    /// Doanh thu tính trên các đơn không bị huỷ.
    /// </summary>
    [HttpGet("revenue/report")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<RevenueReportDto>> GetRevenueReport([FromQuery] int months = 12)
    {
        months = Math.Clamp(months, 1, 36);
        var from = new DateTime(AppTime.Today.Year, AppTime.Today.Month, 1).AddMonths(-(months - 1));

        var bookings = await _context.Bookings
            .Select(b => new
            {
                b.TotalPrice,
                b.Status,
                Date = b.CreatedAt ?? b.CheckIn,
                PropertyId = b.Room.PropertyId,
                PropertyName = b.Room.Property.Name,
                City = b.Room.Property.City,
            })
            .ToListAsync();

        var active = bookings.Where(b => !BookingStatus.Is(b.Status, BookingStatus.Cancelled)).ToList();

        var monthly = Enumerable.Range(0, months)
            .Select(i => from.AddMonths(i))
            .Select(m => new MonthlyRevenueDto
            {
                Month = m.ToString("yyyy-MM"),
                Revenue = active.Where(b => b.Date.Year == m.Year && b.Date.Month == m.Month).Sum(b => b.TotalPrice),
                Bookings = active.Count(b => b.Date.Year == m.Year && b.Date.Month == m.Month),
            })
            .ToList();

        var top = active
            .GroupBy(b => new { b.PropertyId, b.PropertyName, b.City })
            .Select(g => new PropertyRevenueDto
            {
                PropertyId = g.Key.PropertyId,
                PropertyName = g.Key.PropertyName,
                City = g.Key.City,
                Revenue = g.Sum(b => b.TotalPrice),
                Bookings = g.Count(),
            })
            .OrderByDescending(p => p.Revenue)
            .Take(10)
            .ToList();

        var total = active.Sum(b => b.TotalPrice);
        return Ok(new RevenueReportDto
        {
            TotalRevenue = total,
            TotalBookings = bookings.Count,
            CompletedBookings = bookings.Count(b => BookingStatus.Is(b.Status, BookingStatus.Completed)),
            CancelledBookings = bookings.Count(b => BookingStatus.Is(b.Status, BookingStatus.Cancelled)),
            AverageBookingValue = active.Count > 0 ? Math.Round(total / active.Count, 0) : 0,
            Monthly = monthly,
            TopProperties = top,
        });
    }

    /// <summary>
    /// API partner-stats: Lấy thống kê tổng quan cho Dashboard Partner
    /// </summary>
    [HttpGet("partner-stats")]
    [Authorize(Roles = AppRoles.Partner)]
    public async Task<IActionResult> GetPartnerDashboardStats()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized(new { message = "Bạn cần đăng nhập để thực hiện thao tác này" });
        }

        var partner = await _context.Partners.FirstOrDefaultAsync(p => p.UserId == userId);
        if (partner == null) return NotFound("Partner not found");

        var partnerId = partner.Id;

        // 1. Doanh thu của partner: chỉ đơn đã hoàn tất, khớp với số dư ở /finance/business (SRS 2d)
        var totalRevenue = await _context.Bookings
            .Where(b => b.Room.Property.PartnerId == partnerId && b.Status == BookingStatus.Completed)
            .SumAsync(b => (decimal?)(b.TotalPrice - (b.ServiceFee ?? 0))) ?? 0;

        // 2. Tỷ lệ lấp đầy hôm nay của partner
        var today = AppTime.Today;
        var partnerRoomsActive = await _context.Rooms
            .Where(r => r.Property.PartnerId == partnerId)
            .CountAsync();
            
        var bookedRoomsCountActive = await _context.Bookings
            .Where(b => b.Room.Property.PartnerId == partnerId && b.CheckIn <= today && b.CheckOut >= today && b.Status != "Hủy" && b.Status != "Cancelled")
            .Select(b => b.RoomId)
            .Distinct()
            .CountAsync();
            
        double occupancyRateValue = partnerRoomsActive > 0 ? ((double)bookedRoomsCountActive / partnerRoomsActive) * 100 : 0;

        // 3. Số booking active (không phải đã xong hoặc đã hủy)
        var activeBookingStatuses = new[] { "Confirmed", "Checked_in", "confirmed", "checked_in" };
        var activeBookingsNum = await _context.Bookings
            .Where(b => b.Room.Property.PartnerId == partnerId && activeBookingStatuses.Contains(b.Status))
            .CountAsync();

        // 4. Điểm đánh giá trung bình trên các phòng của partner
        var ratings = await _context.Reviews
            .Where(r => r.Room.Property.PartnerId == partnerId && !r.IsHidden)
            .Select(r => (double?)r.Stars)
            .ToListAsync();

        return Ok(new
        {
            TotalRevenue = totalRevenue,
            OccupancyRate = Math.Round(occupancyRateValue, 2),
            ActiveBookingsCount = activeBookingsNum,
            TotalRooms = partnerRoomsActive,
            AverageRating = ratings.Count > 0 ? Math.Round(ratings.Average() ?? 0, 2) : (double?)null,
            TotalReviews = ratings.Count
        });
    }
}
