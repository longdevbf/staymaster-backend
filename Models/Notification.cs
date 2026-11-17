namespace HotelManagement.API.Models;

/// <summary>Thông báo trong app (bảng thong_bao có sẵn trong DB)</summary>
public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    /// <summary>booking | inbox | payout | room | review | system</summary>
    public string? Type { get; set; }
    public bool? IsRead { get; set; } = false;
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    /// <summary>Đường dẫn trong app khi bấm vào thông báo, ví dụ /my-bookings</summary>
    public string? Link { get; set; }

    public User User { get; set; } = null!;
}
