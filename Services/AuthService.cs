using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.DTOs.Auth;
using HotelManagement.API.Models;
using HotelManagement.API.Services.Email;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterCustomerAsync(RegisterCustomerRequest req);
    Task<AuthResponse?> RegisterPartnerAsync(RegisterPartnerRequest req);
    Task<AuthResponse?> LoginAsync(LoginRequest req);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest req);
    Task RequestPasswordResetAsync(string email);
    Task<string?> ResetPasswordAsync(ResetPasswordRequest req);
    Task EnsureAdminUserAsync(string email, string password);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwt;
    private readonly ILogger<AuthService> _logger;
    private readonly INotificationService _notifications;
    private readonly IConfiguration _config;
    private readonly IAuditService _audit;

    private static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromMinutes(30);
    private const int MaxResetRequestsPerWindow = 3;
    private static readonly TimeSpan ResetRequestWindow = TimeSpan.FromMinutes(15);

    public AuthService(AppDbContext db, IJwtService jwt, ILogger<AuthService> logger,
        INotificationService notifications, IConfiguration config, IAuditService audit)
    {
        _audit = audit;
        _db = db;
        _jwt = jwt;
        _logger = logger;
        _notifications = notifications;
        _config = config;
    }

    public async Task<AuthResponse?> RegisterCustomerAsync(RegisterCustomerRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            throw new Exception("Email đã được sử dụng bởi một tài khoản khác.");
        
        if (await _db.Users.AnyAsync(u => u.Phone == req.Phone))
            throw new Exception("Số điện thoại đã được sử dụng bởi một tài khoản khác.");

        var user = new User
        {
            FullName = req.FullName,
            Email = req.Email,
            Phone = req.Phone,
            Username = req.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Cccd = req.Cccd,
            Status = "active",
            CreatedAt = DateTime.Now
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Gán role customer
        await AssignRoleAsync(user.Id, "customer");

        return BuildAuthResponse(user, "customer");
    }

    public async Task<AuthResponse?> RegisterPartnerAsync(RegisterPartnerRequest req)
    {
        // Kiểm tra Email hoặc Số điện thoại đã tồn tại chưa
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
        {
            throw new Exception("Email đã được sử dụng bởi một tài khoản khác.");
        }
        if (await _db.Users.AnyAsync(u => u.Phone == req.Phone))
        {
            throw new Exception("Số điện thoại đã được sử dụng bởi một tài khoản khác.");
        }

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var user = new User
            {
                FullName = req.FullName,
                Email = req.Email,
                Phone = req.Phone,
                Username = req.Email, // Email làm username mặc định (Phải có vì là Unique)
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Status = "active",
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Gán role partner
            await AssignRoleAsync(user.Id, "partner");

            // Tạo bản ghi đối tác (doi_tac) với mã định danh duy nhất để tránh lỗi Unique Constraint
            var partnerCode = "PARTNER-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var partner = new Partner
            {
                UserId = user.Id,
                PartnerCode = partnerCode,
                BusinessName = req.FullName,
                BusinessType = "Cá nhân",
                Status = "active",
                CreatedAt = DateTime.Now
            };
            _db.Partners.Add(partner);
            await _db.SaveChangesAsync();

            // Tạo cơ sở lưu trú mặc định cho đối tác mới
            var property = new Property
            {
                PartnerId = partner.Id,
                Name = req.FullName,
                Type = "Khách sạn",
                City = "Chưa cập nhật",
                District = "Chưa cập nhật",
                Ward = "Chưa cập nhật",
                DetailedAddress = "Chưa cập nhật",
                Latitude = 0,
                Longitude = 0,
                DefaultCheckInTime = new TimeSpan(14, 0, 0), // 14:00
                DefaultCheckOutTime = new TimeSpan(12, 0, 0), // 12:00
                Status = "active",
                CreatedAt = DateTime.Now
            };

            _db.Properties.Add(property);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
            _logger.LogInformation("Partner registered: user {UserId}, partner {PartnerId}", user.Id, partner.Id);
            return BuildAuthResponse(user, "partner");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Partner registration failed for {Email}", req.Email);
            throw new Exception("Đã xảy ra lỗi khi đăng ký đối tác. Vui lòng thử lại.", ex);
        }
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest req)
    {
        // Đăng nhập bằng email hoặc số điện thoại (SRS 2a)
        var identifier = req.Email.Trim();
        var users = _db.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role);
        User? user;
        if (identifier.Contains('@'))
        {
            user = await users.FirstOrDefaultAsync(u => u.Email == identifier);
        }
        else
        {
            var phones = PhoneVariants(identifier);
            user = phones.Count == 0 ? null : await users.FirstOrDefaultAsync(u => u.Phone != null && phones.Contains(u.Phone));
        }

        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
            return null;

        if (UserStatus.IsLocked(user.Status))
            throw new AccountLockedException();

        user.LastActive = DateTime.Now;
        await _db.SaveChangesAsync();

        // Vai trò cao nhất, chuẩn hoá về admin / partner / customer
        var role = AppRoles.Highest(user.UserRoles.Select(ur => ur.Role.RoleName));

        return BuildAuthResponse(user, role);
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest req)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return false;

        if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, user.Password))
            throw new Exception("Mật khẩu hiện tại không chính xác.");

        user.Password = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        user.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        await _audit.LogAsync("PASSWORD_CHANGE", "nguoi_dung", userId, userId: userId);
        return true;
    }

    /// <summary>Tạo tài khoản admin đầu tiên nếu chưa có (thông tin lấy từ cấu hình SeedAdmin).</summary>
    public async Task EnsureAdminUserAsync(string email, string password)
    {
        // Kiểm tra xem đã có admin nào chưa
        var adminRole = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == "admin");
        if (adminRole == null) 
        {
            adminRole = new Role { RoleName = "admin" };
            _db.Roles.Add(adminRole);
            await _db.SaveChangesAsync();
        }

        var adminExists = await _db.Users.AnyAsync(u => u.Email == email);
        if (!adminExists)
        {
            var adminUser = new User
            {
                FullName = "System Administrator",
                Email = email,
                Username = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Status = "active",
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(adminUser);
            await _db.SaveChangesAsync();

            await AssignRoleAsync(adminUser.Id, "admin");
            _logger.LogInformation("Seed admin account created: {Email}", email);
        }
    }

    public async Task RequestPasswordResetAsync(string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email.Trim());
        if (user is null)
        {
            _logger.LogInformation("Password reset requested for unknown email");
            return; // không tiết lộ email có tồn tại hay không
        }

        var now = DateTime.Now;
        var recent = await _db.PasswordResetTokens.CountAsync(t => t.UserId == user.Id && t.CreatedAt > now - ResetRequestWindow);
        if (recent >= MaxResetRequestsPerWindow)
        {
            _logger.LogWarning("Password reset rate limit hit for user {UserId}", user.Id);
            return;
        }

        // Vô hiệu các mã cũ chưa dùng
        var open = await _db.PasswordResetTokens.Where(t => t.UserId == user.Id && t.UsedAt == null && t.ExpiresAt > now).ToListAsync();
        foreach (var t in open) t.UsedAt = now;

        var token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        _db.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = HashToken(token),
            ExpiresAt = now + ResetTokenLifetime,
            CreatedAt = now,
        });
        await _db.SaveChangesAsync();

        var frontend = (_config["App:FrontendUrl"] ?? "http://localhost:8081").TrimEnd('/');
        _notifications.NotifyPasswordReset(user.Email, user.FullName, $"{frontend}/reset-password?token={token}");
        _logger.LogInformation("Password reset email queued for user {UserId}", user.Id);
    }

    /// <summary>Trả về null nếu thành công, ngược lại là thông báo lỗi.</summary>
    public async Task<string?> ResetPasswordAsync(ResetPasswordRequest req)
    {
        var hash = HashToken(req.Token.Trim());
        var token = await _db.PasswordResetTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.TokenHash == hash);
        if (token is null || token.UsedAt != null || token.ExpiresAt < DateTime.Now)
            return "Link đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";

        token.UsedAt = DateTime.Now;
        token.User.Password = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        token.User.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Password reset completed for user {UserId}", token.UserId);
        await _audit.LogAsync("PASSWORD_RESET", "nguoi_dung", token.UserId, userId: token.UserId);
        return null;
    }

    // ── Helpers ───────────────────────────────────────────────
    private static string HashToken(string token) =>
        Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));

    /// <summary>"0912 345 678", "+84912345678", "84912345678" đều khớp với số lưu dạng "0912345678" hoặc "+84...".</summary>
    public static List<string> PhoneVariants(string input)
    {
        var digits = new string(input.Where(char.IsDigit).ToArray());
        if (digits.Length < 8) return new List<string>();
        var local = digits.StartsWith("84") && digits.Length >= 11 ? "0" + digits[2..] : digits;
        var national = local.StartsWith('0') ? local[1..] : local;
        return new List<string> { local, "+84" + national, "84" + national, input.Trim() }.Distinct().ToList();
    }

    // ── Private helpers ───────────────────────────────────────
    private async Task AssignRoleAsync(int userId, string roleName)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
        if (role is null) return;

        _db.UserRoles.Add(new UserRole
        {
            UserId = userId,
            RoleId = role.Id
        });
        
        await _db.SaveChangesAsync();
    }

    private AuthResponse BuildAuthResponse(User user, string role) => new()
    {
        Token = _jwt.GenerateToken(user, role),
        User = new UserInfo
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Avatar = user.Avatar,
            Role = role
        }
    };
}

/// <summary>Đúng mật khẩu nhưng tài khoản đã bị admin khoá.</summary>
public class AccountLockedException : Exception
{
    public AccountLockedException() : base("Tài khoản đã bị khoá") { }
}
