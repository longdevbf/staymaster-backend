namespace HotelManagement.API.Models;

/// <summary>Phòng khách đã lưu (bảng yeu_thich có sẵn trong DB)</summary>
public class Favorite
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    public User User { get; set; } = null!;
    public Room Room { get; set; } = null!;
}
