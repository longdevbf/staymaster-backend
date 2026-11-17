namespace HotelManagement.API.Models;

/// <summary>Mã đặt lại mật khẩu (bảng ma_dat_lai_mat_khau). Chỉ lưu SHA-256 của mã, không lưu mã gốc.</summary>
public class PasswordResetToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User User { get; set; } = null!;
}
