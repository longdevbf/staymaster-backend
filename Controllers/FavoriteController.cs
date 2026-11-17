using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.Models;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Controllers;

/// <summary>Phòng khách đã lưu (nút tim).</summary>
[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoriteController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IRoomService _rooms;

    public FavoriteController(AppDbContext db, IRoomService rooms)
    {
        _db = db;
        _rooms = rooms;
    }

    // GET /api/favorites/ids — id các phòng đã lưu (để tô tim)
    [HttpGet("ids")]
    public async Task<IActionResult> Ids()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        return Ok(await _db.Favorites.Where(f => f.UserId == userId).Select(f => f.RoomId).ToListAsync());
    }

    // GET /api/favorites — phòng đã lưu, mới nhất trước (bỏ phòng không còn mở bán)
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        var ids = await _db.Favorites.Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt).Select(f => f.RoomId).ToListAsync();
        var result = new List<object>();
        foreach (var id in ids)
        {
            var room = await _rooms.GetRoomByIdAsync(id);
            if (room is not null && room.IsApproved == true && room.Status is not ("hidden" or "rejected" or "deleted"))
                result.Add(room);
        }
        return Ok(result);
    }

    // POST /api/favorites/{roomId}
    [HttpPost("{roomId:int}")]
    public async Task<IActionResult> Add(int roomId)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        if (!await _db.Rooms.AnyAsync(r => r.Id == roomId)) return NotFound(new { message = "Phòng không tồn tại" });
        if (!await _db.Favorites.AnyAsync(f => f.UserId == userId && f.RoomId == roomId))
        {
            _db.Favorites.Add(new Favorite { UserId = userId.Value, RoomId = roomId, CreatedAt = DateTime.Now });
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Bấm hai lần cùng lúc: ràng buộc unique đã chặn, coi như đã lưu
            }
        }
        return Ok(new { saved = true });
    }

    // DELETE /api/favorites/{roomId}
    [HttpDelete("{roomId:int}")]
    public async Task<IActionResult> Remove(int roomId)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        await _db.Favorites.Where(f => f.UserId == userId && f.RoomId == roomId).ExecuteDeleteAsync();
        return Ok(new { saved = false });
    }
}
