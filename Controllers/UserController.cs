using System.Security.Claims;
using HotelManagement.API.Common;
using HotelManagement.API.DTOs.User;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuditService _audit;
    private readonly IInAppNotifier _inApp;

    public UserController(IUserService userService, IAuditService audit, IInAppNotifier inApp)
    {
        _userService = userService;
        _audit = audit;
        _inApp = inApp;
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
            return Unauthorized();

        var profile = await _userService.GetUserProfileAsync(userId);
        if (profile == null) return NotFound(new { message = "Người dùng không tồn tại" });

        return Ok(profile);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
            return Unauthorized();

        var updatedProfile = await _userService.UpdateProfileAsync(userId, req);
        if (updatedProfile == null) return NotFound(new { message = "Người dùng không tồn tại" });

        return Ok(updatedProfile);
    }

    // PUT /api/users/{id}/lock  { reason } — khoá tài khoản (không xoá, giữ lịch sử đơn)
    [HttpPut("{id:int}/lock")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> LockUser(int id, [FromBody] LockUserRequest? req = null)
    {
        if (User.GetUserId() == id)
            return BadRequest(new { message = "Không thể khoá tài khoản của chính bạn" });
        var target = await _userService.GetUserProfileAsync(id);
        if (target is null) return NotFound(new { message = "Người dùng không tồn tại" });
        if (target.Role == AppRoles.Admin)
            return BadRequest(new { message = "Không thể khoá tài khoản quản trị viên" });

        await _userService.SetStatusAsync(id, UserStatus.Locked);
        await _audit.LogAsync("USER_LOCK", "nguoi_dung", id, new { status = target.Status },
            new { status = UserStatus.Locked, reason = req?.Reason });
        return Ok(new { message = $"Đã khoá tài khoản {target.FullName}" });
    }

    // PUT /api/users/{id}/unlock
    [HttpPut("{id:int}/unlock")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> UnlockUser(int id)
    {
        var target = await _userService.GetUserProfileAsync(id);
        if (target is null) return NotFound(new { message = "Người dùng không tồn tại" });
        await _userService.SetStatusAsync(id, UserStatus.Active);
        await _audit.LogAsync("USER_UNLOCK", "nguoi_dung", id, new { status = target.Status }, new { status = UserStatus.Active });
        return Ok(new { message = $"Đã mở khoá tài khoản {target.FullName}" });
    }

    // PUT /api/users/{id}/reset-password — người dùng quên mật khẩu và liên hệ hỗ trợ: admin cấp mật khẩu tạm,
    // báo cho người dùng qua điện thoại; người dùng đăng nhập rồi đổi mật khẩu trong Hồ sơ. Không cần email.
    [HttpPut("{id:int}/reset-password")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> ResetPassword(int id)
    {
        if (User.GetUserId() == id)
            return BadRequest(new { message = "Đổi mật khẩu của bạn trong trang Hồ sơ" });
        var target = await _userService.GetUserProfileAsync(id);
        if (target is null) return NotFound(new { message = "Người dùng không tồn tại" });
        if (target.Role == AppRoles.Admin)
            return BadRequest(new { message = "Không cấp lại mật khẩu cho tài khoản quản trị viên khác" });

        var temp = await _userService.ResetPasswordAsync(id);
        if (temp is null) return NotFound(new { message = "Người dùng không tồn tại" });
        // Không ghi mật khẩu vào nhật ký
        await _audit.LogAsync("USER_RESET_PASSWORD", "nguoi_dung", id, null, new { target.FullName, target.Email });
        await _inApp.NotifyAsync(id, "Mật khẩu của bạn đã được cấp lại",
            "Quản trị viên vừa cấp mật khẩu tạm cho tài khoản của bạn. Hãy đổi sang mật khẩu mới trong Hồ sơ.",
            NotificationType.System, "/profile?tab=security");
        return Ok(new { temporaryPassword = temp, message = $"Đã cấp mật khẩu tạm cho {target.FullName}" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(currentUserIdString, out int currentUserId) && currentUserId == id)
        {
            return BadRequest(new { message = "Không thể xóa tài khoản của chính bạn đang đăng nhập" });
        }

        var before = await _userService.GetUserProfileAsync(id);
        var success = await _userService.DeleteUserAsync(id);
        if (!success) return NotFound(new { message = "Người dùng không tồn tại" });
        await _audit.LogAsync("USER_DELETE", "nguoi_dung", id, new { before?.FullName, before?.Email, before?.Role });

        return Ok(new { message = "Đã xóa người dùng thành công" });
    }
}

public class LockUserRequest
{
    public string? Reason { get; set; }
}
