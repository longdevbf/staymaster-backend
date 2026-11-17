using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.Models;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Controllers;

public class CreateRoomBlockRequest
{
    public DateTime From { get; set; }
    /// <summary>Ngày đầu tiên phòng mở lại (không tính)</summary>
    public DateTime To { get; set; }
    public string? Reason { get; set; }
}

/// <summary>Công cụ cho chủ nhà: báo cáo doanh thu theo tháng, lịch phòng, chặn ngày.</summary>
[ApiController]
[Authorize(Roles = AppRoles.PartnerOrAdmin)]
public class PartnerToolsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;
    private readonly ISystemSettings _settings;

    public PartnerToolsController(AppDbContext db, IAuditService audit, ISystemSettings settings)
    {
        _db = db;
        _audit = audit;
        _settings = settings;
    }

    private async Task<int?> PartnerIdAsync()
    {
        var userId = User.GetUserId();
        return userId is null ? null : await _db.Partners.Where(p => p.UserId == userId).Select(p => (int?)p.Id).FirstOrDefaultAsync();
    }

    // GET /api/partner/revenue-report?months=12 — tiền chủ nhà nhận theo tháng trả phòng, theo phòng
    [HttpGet("api/partner/revenue-report")]
    [Authorize(Roles = AppRoles.Partner)]
    public async Task<IActionResult> RevenueReport([FromQuery] int months = 12)
    {
        var partnerId = await PartnerIdAsync();
        if (partnerId is null) return NotFound(new { message = "Bạn chưa đăng ký thông tin đối tác" });
        months = Math.Clamp(months, 1, 24);
        var start = new DateTime(AppTime.Today.Year, AppTime.Today.Month, 1).AddMonths(-(months - 1));

        var rows = await _db.Bookings
            .Where(b => b.Room.Property.PartnerId == partnerId && b.Status == BookingStatus.Completed && b.CheckOut >= start)
            .Select(b => new
            {
                b.CheckOut,
                b.Nights,
                b.RoomId,
                RoomName = b.Room.Name,
                Earning = b.TotalPrice - (b.ServiceFee ?? 0),
                Fee = b.ServiceFee ?? 0,
                b.TotalPrice,
            })
            .ToListAsync();
        var cancelled = await _db.Bookings
            .Where(b => b.Room.Property.PartnerId == partnerId && b.CheckIn >= start &&
                        (b.Status == BookingStatus.Cancelled || b.Status == BookingStatus.CancelledVi))
            .CountAsync();
        var roomCount = await _db.Rooms.CountAsync(r => r.Property.PartnerId == partnerId && r.Status != "deleted");

        var monthly = Enumerable.Range(0, months).Select(i => start.AddMonths(i)).Select(m =>
        {
            var inMonth = rows.Where(r => r.CheckOut.Year == m.Year && r.CheckOut.Month == m.Month).ToList();
            var daysInMonth = DateTime.DaysInMonth(m.Year, m.Month);
            return new
            {
                Month = $"{m:yyyy-MM}",
                Earning = inMonth.Sum(r => r.Earning),
                Bookings = inMonth.Count,
                Nights = inMonth.Sum(r => r.Nights),
                Occupancy = roomCount == 0 ? 0 : Math.Round(inMonth.Sum(r => r.Nights) * 100.0 / (roomCount * daysInMonth), 1),
            };
        }).ToList();

        var byRoom = rows.GroupBy(r => new { r.RoomId, r.RoomName })
            .Select(g => new { g.Key.RoomId, g.Key.RoomName, Earning = g.Sum(r => r.Earning), Bookings = g.Count(), Nights = g.Sum(r => r.Nights) })
            .OrderByDescending(r => r.Earning)
            .ToList();

        return Ok(new
        {
            From = start,
            TotalEarning = rows.Sum(r => r.Earning),
            TotalGuestPaid = rows.Sum(r => r.TotalPrice),
            TotalPlatformFee = rows.Sum(r => r.Fee),
            TotalBookings = rows.Count,
            CancelledBookings = cancelled,
            AverageNightly = rows.Sum(r => r.Nights) == 0 ? 0 : Math.Round(rows.Sum(r => r.Earning) / rows.Sum(r => r.Nights), 0),
            Monthly = monthly,
            ByRoom = byRoom,
        });
    }

    // GET /api/partner/calendar?from=2026-11-01&days=31 — lịch phòng × ngày
    [HttpGet("api/partner/calendar")]
    [Authorize(Roles = AppRoles.Partner)]
    public async Task<IActionResult> Calendar([FromQuery] DateTime? from, [FromQuery] int days = 31)
    {
        var partnerId = await PartnerIdAsync();
        if (partnerId is null) return NotFound(new { message = "Bạn chưa đăng ký thông tin đối tác" });
        days = Math.Clamp(days, 7, 62);
        var start = (from ?? AppTime.Today).Date;
        var end = start.AddDays(days);
        var holdCutoff = DateTime.Now.AddMinutes(-(await _settings.GetAsync()).PaymentHoldMinutes);

        var rooms = await _db.Rooms
            .Where(r => r.Property.PartnerId == partnerId && r.Status != "deleted")
            .OrderBy(r => r.Name)
            .Select(r => new { r.Id, r.Name, r.RoomCode, r.Status, r.IsApproved, r.BasePrice, r.WeekendPrice })
            .ToListAsync();
        var roomIds = rooms.Select(r => r.Id).ToList();

        var bookings = await _db.Bookings
            .Where(b => roomIds.Contains(b.RoomId) && b.CheckIn < end && b.CheckOut > start &&
                        b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.CancelledVi &&
                        !(b.Status == BookingStatus.Pending && b.CreatedAt < holdCutoff))
            .Select(b => new { b.Id, b.RoomId, b.CheckIn, b.CheckOut, b.Status, GuestName = b.User.FullName, b.GuestCount })
            .ToListAsync();
        var blocks = await _db.RoomBlocks
            .Where(b => roomIds.Contains(b.RoomId) && b.FromDate < end && b.ToDate > start)
            .Select(b => new { b.Id, b.RoomId, b.FromDate, b.ToDate, b.Reason })
            .ToListAsync();

        return Ok(new
        {
            From = start,
            Days = days,
            Rooms = rooms.Select(r => new
            {
                r.Id,
                r.Name,
                r.RoomCode,
                r.Status,
                r.IsApproved,
                r.BasePrice,
                r.WeekendPrice,
                Bookings = bookings.Where(b => b.RoomId == r.Id).Select(b => new
                {
                    b.Id, b.CheckIn, b.CheckOut, Status = BookingStatus.Normalize(b.Status), b.GuestName, b.GuestCount,
                }),
                Blocks = blocks.Where(b => b.RoomId == r.Id).Select(b => new { b.Id, From = b.FromDate, To = b.ToDate, b.Reason }),
            }),
        });
    }

    // POST /api/rooms/{roomId}/blocks — chặn ngày (bảo trì, dùng riêng)
    [HttpPost("api/rooms/{roomId:int}/blocks")]
    public async Task<IActionResult> CreateBlock(int roomId, [FromBody] CreateRoomBlockRequest req)
    {
        if (!await CanManageRoomAsync(roomId)) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Bạn không quản lý phòng này" });
        var from = req.From.Date;
        var to = req.To.Date;
        if (from >= to) return BadRequest(new { message = "Ngày bắt đầu phải trước ngày kết thúc" });
        if (to < AppTime.Today) return BadRequest(new { message = "Không chặn được ngày đã qua" });
        if ((to - from).TotalDays > 366) return BadRequest(new { message = "Mỗi lần chặn tối đa 1 năm" });
        var reason = string.IsNullOrWhiteSpace(req.Reason) ? null : req.Reason.Trim();
        if (reason is { Length: > 255 }) return BadRequest(new { message = "Lý do tối đa 255 ký tự" });

        var holdCutoff = DateTime.Now.AddMinutes(-(await _settings.GetAsync()).PaymentHoldMinutes);
        var clash = await _db.Bookings
            .Where(b => b.RoomId == roomId && b.CheckIn < to && b.CheckOut > from &&
                        b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.CancelledVi &&
                        !(b.Status == BookingStatus.Pending && b.CreatedAt < holdCutoff))
            .Select(b => b.Id)
            .FirstOrDefaultAsync();
        if (clash != 0) return BadRequest(new { message = $"Đã có đơn #{clash} trong khoảng ngày này" });

        var block = new RoomBlock { RoomId = roomId, FromDate = from, ToDate = to, Reason = reason, CreatedAt = DateTime.Now };
        _db.RoomBlocks.Add(block);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("ROOM_BLOCK", "ngay_chan_phong", block.Id, null, new { roomId, from, to, reason });
        return Ok(new { block.Id, From = block.FromDate, To = block.ToDate, block.Reason });
    }

    // DELETE /api/rooms/{roomId}/blocks/{id}
    [HttpDelete("api/rooms/{roomId:int}/blocks/{id:int}")]
    public async Task<IActionResult> DeleteBlock(int roomId, int id)
    {
        if (!await CanManageRoomAsync(roomId)) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Bạn không quản lý phòng này" });
        var block = await _db.RoomBlocks.FirstOrDefaultAsync(b => b.Id == id && b.RoomId == roomId);
        if (block is null) return NotFound(new { message = "Không tìm thấy ngày chặn" });
        _db.RoomBlocks.Remove(block);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("ROOM_UNBLOCK", "ngay_chan_phong", id, new { roomId, block.FromDate, block.ToDate });
        return NoContent();
    }

    private async Task<bool> CanManageRoomAsync(int roomId)
    {
        if (User.IsAdmin()) return await _db.Rooms.AnyAsync(r => r.Id == roomId);
        var userId = User.GetUserId();
        return userId is not null && await _db.Rooms.AnyAsync(r => r.Id == roomId && r.Property.Partner.UserId == userId);
    }
}
