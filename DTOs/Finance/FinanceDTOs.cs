namespace HotelManagement.API.DTOs.Finance;

public class BusinessFinanceSummary
{
    /// <summary>Doanh thu từ các đơn đã hoàn tất.</summary>
    public decimal TotalEarnings { get; set; }
    /// <summary>Đơn đã xác nhận / đang lưu trú, sẽ cộng khi hoàn tất.</summary>
    public decimal UpcomingEarnings { get; set; }
    public decimal PendingPayouts { get; set; }
    public decimal AvailableBalance { get; set; }
    public List<FinanceTransactionDto> RecentTransactions { get; set; } = new();
}

public class AdminFinanceSummary
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalPayouts { get; set; }
    /// <summary>Phí dịch vụ StayMaster thu từ các đơn đã hoàn tất</summary>
    public decimal PlatformFees { get; set; }
    public int TotalBookings { get; set; }
    public int TotalUsers { get; set; }
    public int TotalProperties { get; set; }
    public List<FinanceTransactionDto> SystemTransactions { get; set; } = new();
}

public class FinanceTransactionDto
{
    public int Id { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
}
