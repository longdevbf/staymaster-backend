using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.DTOs.Room;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly AppDbContext _db;
    private readonly ILogger<RoomController> _logger;
    private readonly IAuditService _audit;
    private readonly IInAppNotifier _inApp;

    public RoomController(IRoomService roomService, AppDbContext db, ILogger<RoomController> logger, IAuditService audit,
        IInAppNotifier inApp)
    {
        _inApp = inApp;
        _roomService = roomService;
        _db = db;
        _logger = logger;
        _audit = audit;
    }

    // GET /api/rooms  — phòng đang hiển thị công khai
    [HttpGet]
    public async Task<IActionResult> GetAllRooms()
    {
        var rooms = await _roomService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    // GET /api/rooms/availability?checkIn=2026-12-01&checkOut=2026-12-03
    // Trả về id các phòng đã kín trong khoảng ngày; không lộ thông tin đơn đặt phòng.
    [HttpGet("availability")]
    public async Task<IActionResult> GetAvailability([FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
    {
        if (checkIn >= checkOut)
            return BadRequest(new { message = "Ngày nhận phòng phải trước ngày trả phòng" });

        var ids = await _roomService.GetUnavailableRoomIdsAsync(checkIn, checkOut);
        return Ok(new { unavailableRoomIds = ids });
    }

    // GET /api/rooms/admin  (Admin)
    [HttpGet("admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAdminRooms()
    {
        var rooms = await _roomService.GetAdminRoomsAsync();
        return Ok(rooms);
    }

    // GET /api/rooms/partner/{partnerUserId}  (Admin)
    [HttpGet("partner/{partnerId:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetPartnerRooms(int partnerId)
    {
        var rooms = await _roomService.GetPartnerRoomsAsync(partnerId);
        return Ok(rooms);
    }

    // GET /api/rooms/my-listings  (Đối tác)
    [HttpGet("my-listings")]
    [Authorize(Roles = AppRoles.Partner)]
    public async Task<IActionResult> GetMyListings()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized(new { message = "Không thể xác định danh tính đối tác" });

        var rooms = await _roomService.GetPartnerRoomsAsync(userId.Value);
        return Ok(rooms);
    }

    // GET /api/rooms/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRoom(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        if (room is null) return NotFound(new { message = "Không tìm thấy phòng" });

        return Ok(room);
    }

    // POST /api/rooms  (Đối tác: phòng thuộc cơ sở của mình, chờ admin duyệt; Admin: chỉ định PropertyId)
    [HttpPost]
    [Authorize(Roles = AppRoles.PartnerOrAdmin)]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest req)
    {
        if (User.IsPartner() && !User.IsAdmin())
        {
            var propertyId = await GetPartnerPropertyIdAsync();
            if (propertyId is null)
                return BadRequest(new { message = "Bạn chưa có cơ sở lưu trú. Vui lòng liên hệ Admin để khởi tạo khách sạn của bạn." });
            req.PropertyId = propertyId.Value;
        }
        else if (!await _db.Properties.AnyAsync(p => p.Id == req.PropertyId))
        {
            return BadRequest(new { message = "Cơ sở lưu trú không tồn tại" });
        }

        var (room, error) = await _roomService.CreateRoomAsync(req);
        if (error is not null) return BadRequest(new { message = error });

        _logger.LogInformation("Room {RoomId} ({RoomCode}) created for property {PropertyId} by user {UserId}",
            room!.Id, room.RoomCode, room.PropertyId, User.GetUserId());
        await _audit.LogAsync("ROOM_CREATE", "phong", room.Id, null, new { room.Name, room.RoomCode, room.PropertyId, room.BasePrice });
        if (!User.IsAdmin())
            await _inApp.NotifyAdminsAsync("Phòng mới chờ duyệt", $"{room.Name} · {room.PropertyName}", NotificationType.Room, "/admin/listings?tab=pending");
        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
    }

    // PUT /api/rooms/{id}  (Đối tác sở hữu phòng, Admin)
    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.PartnerOrAdmin)]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomRequest req)
    {
        if (!User.IsAdmin())
        {
            var userId = User.GetUserId();
            if (userId is null || !await _roomService.IsOwnedByPartnerUserAsync(id, userId.Value))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Bạn không có quyền chỉnh sửa phòng này" });

            // Đối tác không tự duyệt, đổi trạng thái duyệt hay chuyển phòng sang cơ sở khác
            req.IsApproved = null;
            req.Status = null;
            req.PropertyId = null;

            // Phòng bị từ chối: sửa xong thì tự gửi duyệt lại
            var current = await _db.Rooms.Where(r => r.Id == id).Select(r => new { r.Status, r.Name }).FirstOrDefaultAsync();
            if (current?.Status == "rejected")
            {
                req.Status = "pending";
                await _inApp.NotifyAdminsAsync("Phòng gửi duyệt lại", $"{current.Name} đã được chủ nhà sửa sau khi bị từ chối.",
                    NotificationType.Room, "/admin/listings?tab=pending");
            }
        }

        var (room, error) = await _roomService.UpdateRoomAsync(id, req);
        if (error is not null) return BadRequest(new { message = error });
        if (room is null) return NotFound(new { message = "Không tìm thấy phòng" });
        await _audit.LogAsync("ROOM_UPDATE", "phong", id, null, req);

        return Ok(room);
    }

    // PUT /api/rooms/{id}/approve  (Admin)
    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> ApproveRoom(int id)
    {
        var success = await _roomService.ApproveRoomAsync(id);
        if (!success) return NotFound(new { message = "Không tìm thấy phòng" });
        await _audit.LogAsync("ROOM_APPROVE", "phong", id);
        var name = await _db.Rooms.Where(r => r.Id == id).Select(r => r.Name).FirstOrDefaultAsync();
        await _inApp.NotifyRoomOwnerAsync(id, "Phòng đã được duyệt", $"{name} đã hiển thị cho khách đặt.", NotificationType.Room, $"/rooms/{id}");

        return Ok(new { message = "Duyệt phòng thành công" });
    }

    // PUT /api/rooms/{id}/reject  (Admin)
    [HttpPut("{id:int}/reject")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> RejectRoom(int id, [FromBody] RejectRoomRequest? req = null)
    {
        var reason = req?.Reason?.Trim();
        if (reason is { Length: > 500 }) return BadRequest(new { message = "Lý do tối đa 500 ký tự" });
        var success = await _roomService.RejectRoomAsync(id, reason);
        if (!success) return NotFound(new { message = "Không tìm thấy phòng" });
        await _audit.LogAsync("ROOM_REJECT", "phong", id, null, new { reason });
        var name = await _db.Rooms.Where(r => r.Id == id).Select(r => r.Name).FirstOrDefaultAsync();
        await _inApp.NotifyRoomOwnerAsync(id, "Phòng chưa được duyệt",
            $"{name}: {(string.IsNullOrWhiteSpace(reason) ? "vui lòng kiểm tra lại thông tin và ảnh." : reason)} Sửa phòng để gửi duyệt lại.",
            NotificationType.Room, "/business/listings");

        return Ok(new { message = "Đã từ chối phòng" });
    }

    // PUT /api/rooms/{id}/hide  (Admin) — ẩn phòng đang hoạt động khỏi trang tìm kiếm
    [HttpPut("{id:int}/hide")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> HideRoom(int id)
    {
        var success = await _roomService.SetHiddenAsync(id, hidden: true);
        if (!success) return NotFound(new { message = "Không tìm thấy phòng" });
        await _audit.LogAsync("ROOM_HIDE", "phong", id);
        var name = await _db.Rooms.Where(r => r.Id == id).Select(r => r.Name).FirstOrDefaultAsync();
        await _inApp.NotifyRoomOwnerAsync(id, "Phòng đã bị ẩn", $"{name} tạm ẩn khỏi trang tìm kiếm. Liên hệ StayMaster để biết thêm.",
            NotificationType.Room, "/business/listings");

        return Ok(new { message = "Đã ẩn phòng" });
    }

    // PUT /api/rooms/{id}/show  (Admin)
    [HttpPut("{id:int}/show")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> ShowRoom(int id)
    {
        var success = await _roomService.SetHiddenAsync(id, hidden: false);
        if (!success) return NotFound(new { message = "Không tìm thấy phòng" });
        await _audit.LogAsync("ROOM_SHOW", "phong", id);

        return Ok(new { message = "Đã hiển thị lại phòng" });
    }

    // DELETE /api/rooms/{id}  (Đối tác sở hữu phòng, Admin)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.PartnerOrAdmin)]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        if (!User.IsAdmin())
        {
            var userId = User.GetUserId();
            if (userId is null || !await _roomService.IsOwnedByPartnerUserAsync(id, userId.Value))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Bạn không có quyền xoá phòng này" });
        }

        var success = await _roomService.DeleteRoomAsync(id);
        if (!success) return NotFound(new { message = "Không tìm thấy phòng" });

        _logger.LogInformation("Room {RoomId} deleted by user {UserId}", id, User.GetUserId());
        await _audit.LogAsync("ROOM_DELETE", "phong", id);
        return Ok(new { message = "Xóa phòng thành công" });
    }

    private async Task<int?> GetPartnerPropertyIdAsync()
    {
        var userId = User.GetUserId();
        if (userId is null) return null;

        return await _db.Properties
            .Where(p => p.Partner != null && p.Partner.UserId == userId.Value)
            .Select(p => (int?)p.Id)
            .FirstOrDefaultAsync();
    }
}

public class RejectRoomRequest
{
    public string? Reason { get; set; }
}
