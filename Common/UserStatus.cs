namespace HotelManagement.API.Common;

public static class UserStatus
{
    public const string Active = "active";
    /// <summary>Admin khoá tài khoản: không đăng nhập được, token cũ bị từ chối</summary>
    public const string Locked = "locked";

    public static bool IsLocked(string? status) => string.Equals(status, Locked, StringComparison.OrdinalIgnoreCase);
}
