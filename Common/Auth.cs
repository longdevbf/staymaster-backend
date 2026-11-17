using System.Security.Claims;

namespace HotelManagement.API.Common;

public static class AppRoles
{
    public const string Admin = "admin";
    public const string Partner = "partner";
    public const string Customer = "customer";

    /// <summary>For [Authorize(Roles = ...)]: either role is accepted.</summary>
    public const string PartnerOrAdmin = "partner,admin";

    /// <summary>
    /// Bảng vai_tro chứa nhiều tên cũ ("Admin", "Super Admin", "Khach hang", "Chu doi tac", ...).
    /// Ứng dụng chỉ dùng 3 vai trò chữ thường; tên khác được coi là khách hàng (quyền thấp nhất).
    /// </summary>
    public static string FromDbRoleName(string? roleName) => roleName?.Trim().ToLowerInvariant() switch
    {
        "admin" or "super admin" => Admin,
        "partner" or "chu doi tac" or "chủ đối tác" => Partner,
        _ => Customer,
    };

    private static int Rank(string role) => role switch { Admin => 3, Partner => 2, _ => 1 };

    /// <summary>Vai trò cao nhất của người dùng: admin &gt; partner &gt; customer.</summary>
    public static string Highest(IEnumerable<string?> dbRoleNames) =>
        dbRoleNames.Select(FromDbRoleName).OrderByDescending(Rank).FirstOrDefault() ?? Customer;
}

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user) =>
        int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(AppRoles.Admin);

    public static bool IsPartner(this ClaimsPrincipal user) => user.IsInRole(AppRoles.Partner);
}
