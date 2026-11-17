namespace HotelManagement.API.Common;

/// <summary>
/// Ngày theo giờ Việt Nam. Máy chủ có thể chạy ở múi giờ khác (ví dụ hosting nước ngoài), nên mọi so sánh
/// "hôm nay" với ngày nhận / trả phòng dùng <see cref="Today"/> thay cho DateTime.Today.
/// </summary>
public static class AppTime
{
    private static readonly TimeZoneInfo Zone = FindZone();

    private static TimeZoneInfo FindZone()
    {
        foreach (var id in new[] { "Asia/Ho_Chi_Minh", "SE Asia Standard Time" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }
        return TimeZoneInfo.CreateCustomTimeZone("ICT", TimeSpan.FromHours(7), "Indochina Time", "Indochina Time");
    }

    /// <summary>Giờ hiện tại ở Việt Nam</summary>
    public static DateTime VietnamNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);

    /// <summary>Ngày hôm nay ở Việt Nam</summary>
    public static DateTime Today => VietnamNow.Date;
}
