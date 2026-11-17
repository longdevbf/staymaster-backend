using HotelManagement.API.Common;
using HotelManagement.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Controllers;

/// <summary>Thông báo trong app của người đang đăng nhập (chuông trên đầu trang).</summary>
[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly AppDbContext _db;

    public NotificationController(AppDbContext db) => _db = db;

    // GET /api/notifications?take=20
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int take = 20)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        take = Math.Clamp(take, 1, 100);

        var unread = await _db.Notifications.CountAsync(n => n.UserId == userId && n.IsRead != true);
        var items = await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt).ThenByDescending(n => n.Id)
            .Take(take)
            .Select(n => new { n.Id, n.Title, n.Content, n.Type, n.Link, IsRead = n.IsRead == true, n.CreatedAt })
            .ToListAsync();
        return Ok(new { unread, items });
    }

    // GET /api/notifications/unread-count
    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        return Ok(new { unread = await _db.Notifications.CountAsync(n => n.UserId == userId && n.IsRead != true) });
    }

    // PUT /api/notifications/{id}/read
    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (n is null) return NotFound(new { message = "Không tìm thấy thông báo" });
        n.IsRead = true;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PUT /api/notifications/read-all
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        await _db.Notifications.Where(n => n.UserId == userId && n.IsRead != true)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        return NoContent();
    }
}
