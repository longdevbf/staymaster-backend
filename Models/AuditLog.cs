namespace HotelManagement.API.Models;

/// <summary>Nhật ký thao tác quan trọng (bảng log_he_thong có sẵn trong DB).</summary>
public class AuditLog
{
    public long Id { get; set; }
    public int? UserId { get; set; }
    /// <summary>Mã hành động, ví dụ ROOM_APPROVE, USER_DELETE, LOGIN_FAILED.</summary>
    public string Action { get; set; } = string.Empty;
    public string? TableName { get; set; }
    public string? EntityKey { get; set; }
    public string? OldData { get; set; }
    public string? NewData { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
}
