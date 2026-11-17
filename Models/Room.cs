using System.Text.Json.Serialization;

namespace HotelManagement.API.Models;

public class Room
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string? RoomType { get; set; }
    public int Capacity { get; set; }
    public int? BedCount { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? Area { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; } = "available";
    public bool? IsApproved { get; set; } = false;
    /// <summary>Lý do admin từ chối duyệt (hiện cho đối tác)</summary>
    public string? RejectReason { get; set; }
    /// <summary>Giá mỗi đêm thứ Sáu, thứ Bảy; null = giá cơ bản</summary>
    public decimal? WeekendPrice { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    [JsonIgnore]
    public Property Property { get; set; } = null!;
    public ICollection<RoomImage> Images { get; set; } = new List<RoomImage>();
    public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    public ICollection<RoomBlock> Blocks { get; set; } = new List<RoomBlock>();
}