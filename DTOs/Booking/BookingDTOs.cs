using System.ComponentModel.DataAnnotations;

namespace HotelManagement.API.DTOs.Booking;

public class CreateBookingRequest
{
    [Required] public int RoomId { get; set; }
    [Required] public DateTime CheckIn { get; set; }
    [Required] public DateTime CheckOut { get; set; }
    public int GuestCount { get; set; } = 1;
    public string? Note { get; set; }
    public bool IsDeposit { get; set; } = true; // true = cọc 30%, false = full
}

public class BookingResponse
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Nights { get; set; }
    public int GuestCount { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal? CleaningFee { get; set; }
    public decimal? ServiceFee { get; set; }
    public decimal? Tax { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? Deposit { get; set; }
    public string? Status { get; set; }
    public string? Note { get; set; }
    public string? RoomImage { get; set; }
    public string? UserAvatar { get; set; }
    public string? UserPhone { get; set; }
    public string? UserEmail { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    /// <summary>Tiền chủ nhà nhận khi khách trả phòng (tổng trừ phí dịch vụ)</summary>
    public decimal PartnerEarning { get; set; }
    /// <summary>Đơn chờ thanh toán: hạn chót trước khi tự huỷ</summary>
    public DateTime? PaymentDeadline { get; set; }
    public int PropertyId { get; set; }
    public string? PropertyName { get; set; }
    public string? PropertyAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    /// <summary>Giờ nhận / trả phòng của cơ sở, dạng "14:00"</summary>
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }
    public string? HostName { get; set; }
    /// <summary>Chỉ có khi đơn đã xác nhận hoặc đang lưu trú</summary>
    public string? HostPhone { get; set; }
    public int? ReviewId { get; set; }
    public int? ReviewStars { get; set; }
}

public class CancelBookingRequest
{
    [MaxLength(500)] public string? Reason { get; set; }
}

public class OccupancyRateResponse
{
    public int RoomId { get; set; }
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public double OccupancyRate { get; set; }
    public int TotalDays { get; set; }
    public int OccupiedDays { get; set; }
}