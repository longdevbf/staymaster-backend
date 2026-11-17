namespace HotelManagement.API.Models;

/// <summary>Tiện ích (bảng tien_ich có sẵn trong DB).</summary>
public class Amenity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Group { get; set; }
    /// <summary>Khoá icon ngắn, ví dụ "wifi", "ac", "pool"; frontend đổi sang icon hiển thị.</summary>
    public string? Icon { get; set; }

    public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
}

/// <summary>Bảng nối phong_tien_ich.</summary>
public class RoomAmenity
{
    public int RoomId { get; set; }
    public int AmenityId { get; set; }

    public Room Room { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}
