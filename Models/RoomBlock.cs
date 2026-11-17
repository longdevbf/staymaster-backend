namespace HotelManagement.API.Models;

/// <summary>Khoảng ngày chủ nhà chặn phòng (bảo trì, dùng riêng). ToDate là ngày đầu tiên phòng mở lại.</summary>
public class RoomBlock
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Room Room { get; set; } = null!;
}
