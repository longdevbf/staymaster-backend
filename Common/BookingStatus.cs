namespace HotelManagement.API.Common;

/// <summary>
/// Booking status values written by the API. Old rows in the database also contain
/// "Confirmed", "Completed", "Cancelled" and "Hủy"; <see cref="Normalize"/> maps them.
/// SQL Server uses a case-insensitive collation, so LINQ-to-SQL comparisons against these
/// constants already match the capitalised variants.
/// </summary>
public static class BookingStatus
{
    public const string Pending = "pending";
    public const string Confirmed = "confirmed";
    public const string CheckedIn = "checked_in";
    public const string Completed = "completed";
    public const string Cancelled = "cancelled";

    /// <summary>Legacy spelling of "cancelled" still present in old rows.</summary>
    public const string CancelledVi = "Hủy";

    public static string Normalize(string? status) => status?.Trim().ToLowerInvariant() switch
    {
        null or "" => Pending,
        "hủy" or "huy" or "canceled" => Cancelled,
        "checked-in" or "checkedin" => CheckedIn,
        var s => s,
    };

    public static bool Is(string? status, string expected) => Normalize(status) == expected;
}
