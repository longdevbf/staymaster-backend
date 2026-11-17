using System.Security.Claims;
using HotelManagement.API.DTOs.Auth;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using HotelManagement.API.Services.Email;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAuditService _audit;
    private readonly EmailOptions _emailOptions;

    public AuthController(IAuthService authService, IAuditService audit, IOptions<EmailOptions> emailOptions)
    {
        _authService = authService;
        _audit = audit;
        _emailOptions = emailOptions.Value;
    }

    // POST /api/auth/register/customer
    [HttpPost("register/customer")]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });

        try {
            var result = await _authService.RegisterCustomerAsync(req);
            return Ok(result);
        } catch (Exception ex) {
            return Conflict(new { message = ex.Message });
        }
    }

    // POST /api/auth/register/partner
    [HttpPost("register/partner")]
    public async Task<IActionResult> RegisterPartner([FromBody] RegisterPartnerRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });

        try {
            var result = await _authService.RegisterPartnerAsync(req);
            return Ok(result);
        } catch (Exception ex) {
            // AuthService hiện tại ném Exception cho trường hợp conflict (Email/Phone tồn tại)
            return Conflict(new { message = ex.Message });
        }
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Dữ liệu không hợp lệ" });

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        AuthResponse? result;
        try
        {
            result = await _authService.LoginAsync(req);
        }
        catch (AccountLockedException)
        {
            await _audit.LogAsync("LOGIN_LOCKED", "nguoi_dung", req.Email.Trim(), null, new { ip });
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = "Tài khoản của bạn đã bị khoá. Vui lòng liên hệ StayMaster để được hỗ trợ." });
        }
        if (result is null)
        {
            await _audit.LogAsync("LOGIN_FAILED", "nguoi_dung", req.Email.Trim(), null, new { ip });
            return Unauthorized(new { message = "Email/số điện thoại hoặc mật khẩu không đúng" });
        }

        await _audit.LogAsync("LOGIN", "nguoi_dung", result.User.Id, null, new { ip, role = result.User.Role }, result.User.Id);
        return Ok(result);
    }

    // POST /api/auth/forgot-password — luôn trả 200 để không lộ email nào đã đăng ký
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Email không hợp lệ" });

        if (!_emailOptions.IsConfigured)
            return Ok(new { emailEnabled = false, message = "StayMaster chưa gửi email. Bạn liên hệ hỗ trợ để được cấp mật khẩu tạm." });

        await _authService.RequestPasswordResetAsync(req.Email);
        return Ok(new { message = "Nếu email đã được đăng ký, chúng tôi đã gửi link đặt lại mật khẩu. Link có hiệu lực 30 phút." });
    }

    // POST /api/auth/reset-password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Mật khẩu tối thiểu 6 ký tự và phải khớp với ô xác nhận" });

        var error = await _authService.ResetPasswordAsync(req);
        if (error is not null) return BadRequest(new { message = error });

        return Ok(new { message = "Đặt lại mật khẩu thành công. Bạn có thể đăng nhập bằng mật khẩu mới." });
    }

    // POST /api/auth/change-password
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Dữ liệu không hợp lệ" });

        try {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized(new { message = "Không xác định được người dùng" });

            var result = await _authService.ChangePasswordAsync(userId, req);
            if (result)
                return Ok(new { message = "Đổi mật khẩu thành công" });
            
            return BadRequest(new { message = "Không thể đổi mật khẩu" });
        } catch (Exception ex) {
            return BadRequest(new { message = ex.Message });
        }
    }
}