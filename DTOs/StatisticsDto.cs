namespace HotelManagement.API.DTOs.Statistics;

public class RevenueResponse
{
    public decimal TotalRevenue { get; set; }
}

public class OccupancyRateResponse
{
    public double Percentage { get; set; }
    public int TotalRooms { get; set; }
    public int BookedRooms { get; set; }
}

public class TopBookedRoomDto
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int BookingCount { get; set; }
}

public class RoomRecommendationDto
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int BookingCount { get; set; }
    public double? DistanceKm { get; set; }
    public double Score { get; set; }
    /// <summary>Lý do ngắn hiển thị cho người dùng, ví dụ "Cách bạn 3 km".</summary>
    public string Reason { get; set; } = string.Empty;
}

public class MonthlyRevenueDto
{
    /// <summary>Tháng dạng "yyyy-MM".</summary>
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Bookings { get; set; }
}

public class PropertyRevenueDto
{
    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string? City { get; set; }
    public decimal Revenue { get; set; }
    public int Bookings { get; set; }
}

public class RevenueReportDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public decimal AverageBookingValue { get; set; }
    public List<MonthlyRevenueDto> Monthly { get; set; } = new();
    public List<PropertyRevenueDto> TopProperties { get; set; } = new();
}
