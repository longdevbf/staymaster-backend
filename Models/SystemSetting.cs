namespace HotelManagement.API.Models;

/// <summary>Cấu hình hệ thống admin sửa được (phí dịch vụ, thuế, thời gian giữ phòng...)</summary>
public class SystemSetting
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int? UpdatedBy { get; set; }
}
