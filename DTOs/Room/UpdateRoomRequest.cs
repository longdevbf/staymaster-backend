namespace HotelManagement.API.DTOs.Room;

public class UpdateRoomRequest
{
    public string? Name { get; set; }
    public string? RoomCode { get; set; }
    public string? RoomType { get; set; }
    public int? Capacity { get; set; }
    public int? BedCount { get; set; }
    public decimal? BasePrice { get; set; }
    public decimal? Area { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public bool? IsApproved { get; set; }
    public int? PropertyId { get; set; }
    /// <summary>null = giữ nguyên; danh sách (kể cả rỗng) = thay toàn bộ tiện ích.</summary>
    public List<int>? AmenityIds { get; set; }
    /// <summary>Giá đêm thứ Sáu, thứ Bảy; 0 = bỏ giá cuối tuần; null = giữ nguyên</summary>
    public decimal? WeekendPrice { get; set; }
    /// <summary>null = giữ nguyên; danh sách = thay toàn bộ ảnh (ảnh đầu tiên là ảnh chính).</summary>
    public List<string>? ImageUrls { get; set; }
}
