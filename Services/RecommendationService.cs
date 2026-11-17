using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.DTOs.Statistics;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Services;

public interface IRecommendationService
{
    /// <summary>
    /// Đề xuất tối đa <paramref name="limit"/> phòng đang hiển thị công khai (SRS 2b).
    /// </summary>
    /// <param name="userId">Người dùng đang đăng nhập (nếu có) để dùng lịch sử lưu trú.</param>
    /// <param name="latitude">Vị trí hiện tại (nếu người dùng cho phép).</param>
    Task<List<RoomRecommendationDto>> RecommendAsync(int? userId, double? latitude, double? longitude, int limit = 8);
}

/// <summary>
/// Điểm = tổng có trọng số của 4 thành phần, mỗi thành phần trong [0, 1]:
/// <list type="bullet">
/// <item>gần vị trí hiện tại: 1 / (1 + km / 50)</item>
/// <item>điểm đánh giá trung bình / 5 (chưa có đánh giá = 0.5)</item>
/// <item>lượt đặt / lượt đặt cao nhất</item>
/// <item>lịch sử: cùng thành phố (+0.7) và cùng loại phòng (+0.3) với các lần lưu trú trước</item>
/// </list>
/// Trọng số nằm trong <see cref="Weights"/> để dễ chỉnh mà không đụng phần còn lại.
/// </summary>
public class RecommendationService : IRecommendationService
{
    private record Weights(double Proximity, double Rating, double Popularity, double History);

    private static readonly Weights WithLocation = new(0.35, 0.30, 0.20, 0.15);
    private static readonly Weights WithoutLocation = new(0.00, 0.45, 0.35, 0.20);
    private const double NeutralRating = 0.5;
    private const double DistanceHalfScoreKm = 50;

    private readonly AppDbContext _db;
    public RecommendationService(AppDbContext db) => _db = db;

    public async Task<List<RoomRecommendationDto>> RecommendAsync(int? userId, double? latitude, double? longitude, int limit = 8)
    {
        var rooms = await _db.Rooms
            .Where(r => r.IsApproved == true && r.Status != "rejected" && r.Status != "deleted" && r.Status != "hidden")
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.RoomType,
                City = r.Property.City,
                Lat = r.Property.Latitude,
                Lng = r.Property.Longitude,
                AverageRating = _db.Reviews.Where(rv => rv.RoomId == r.Id && !rv.IsHidden).Average(rv => (double?)rv.Stars),
                BookingCount = _db.Bookings.Count(b => b.RoomId == r.Id &&
                                                       b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.CancelledVi),
            })
            .ToListAsync();
        if (rooms.Count == 0) return new();

        var history = userId is null
            ? new List<(string? City, string? RoomType)>()
            : (await _db.Bookings
                .Where(b => b.UserId == userId && b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.CancelledVi)
                .Select(b => new { b.Room.Property.City, b.Room.RoomType })
                .ToListAsync())
                .Select(h => (City: (string?)h.City, h.RoomType)).ToList();
        var historyCities = history.Select(h => Normalize(h.City)).Where(c => c != "").ToHashSet();
        var historyTypes = history.Select(h => Normalize(h.RoomType)).Where(t => t != "").ToHashSet();

        var hasLocation = latitude is not null && longitude is not null;
        var w = hasLocation ? WithLocation : WithoutLocation;
        var maxBookings = Math.Max(1, rooms.Max(r => r.BookingCount));

        return rooms
            .Select(r =>
            {
                double? distanceKm = hasLocation && r.Lat is not null && r.Lng is not null && (r.Lat != 0 || r.Lng != 0)
                    ? HaversineKm(latitude!.Value, longitude!.Value, (double)r.Lat, (double)r.Lng)
                    : null;
                var proximity = distanceKm is null ? 0 : 1 / (1 + distanceKm.Value / DistanceHalfScoreKm);
                var rating = r.AverageRating is null ? NeutralRating : r.AverageRating.Value / 5;
                var popularity = (double)r.BookingCount / maxBookings;
                var historyScore = (historyCities.Contains(Normalize(r.City)) ? 0.7 : 0) +
                                   (historyTypes.Contains(Normalize(r.RoomType)) ? 0.3 : 0);

                var score = w.Proximity * proximity + w.Rating * rating + w.Popularity * popularity + w.History * historyScore;

                return new RoomRecommendationDto
                {
                    RoomId = r.Id,
                    RoomName = r.Name,
                    AverageRating = Math.Round(r.AverageRating ?? 0, 1),
                    BookingCount = r.BookingCount,
                    DistanceKm = distanceKm is null ? null : Math.Round(distanceKm.Value, 1),
                    Score = Math.Round(score * 100, 2),
                    Reason = BuildReason(distanceKm, r.AverageRating, r.BookingCount, maxBookings, historyScore),
                };
            })
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.RoomId)
            .Take(Math.Clamp(limit, 1, 20))
            .ToList();
    }

    private static string BuildReason(double? distanceKm, double? rating, int bookings, int maxBookings, double historyScore)
    {
        if (distanceKm is < 30) return $"Cách bạn {distanceKm:0.#} km";
        if (historyScore >= 0.7) return "Gần nơi bạn từng ở";
        if (rating is >= 4.5) return $"Đánh giá {rating:0.0}/5";
        if (bookings > 0 && bookings == maxBookings) return "Được đặt nhiều nhất";
        if (rating is not null) return $"Đánh giá {rating:0.0}/5";
        return "Phòng mới";
    }

    public static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371;
        double ToRad(double deg) => deg * Math.PI / 180;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    /// <summary>So khớp không phân biệt hoa thường và dấu ("Hà Nội" == "Ha Noi").</summary>
    public static string Normalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        var decomposed = text.Trim().ToLowerInvariant().Replace('đ', 'd').Normalize(System.Text.NormalizationForm.FormD);
        var chars = decomposed.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
                                          System.Globalization.UnicodeCategory.NonSpacingMark);
        return new string(chars.ToArray());
    }
}
