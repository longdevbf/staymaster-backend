using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Services;

public record ReviewItem(
    int Id,
    int RoomId,
    string RoomName,
    string PropertyName,
    string? City,
    int UserId,
    string UserName,
    string? UserAvatar,
    int Stars,
    string? Comment,
    DateTime? CreatedAt,
    int? BookingId,
    int? Nights,
    string? Reply,
    DateTime? ReplyAt,
    bool IsHidden,
    string? HiddenReason);

public record RoomReviewsResponse(
    int RoomId,
    double? Average,
    int Count,
    int[] Distribution,
    double? PropertyAverage,
    int PropertyCount,
    int Page,
    int PageSize,
    List<ReviewItem> Items);

public record PagedReviews(int Total, int Page, int PageSize, List<ReviewItem> Items);

public interface IReviewService
{
    Task<RoomReviewsResponse> GetRoomReviewsAsync(int roomId, int page, int pageSize);
    Task<List<ReviewItem>> GetFeaturedAsync(int take);
    Task<(ReviewItem? review, string? error)> CreateAsync(int userId, int bookingId, int stars, string? comment);
    Task<(ReviewItem? review, string? error)> ReplyAsync(int reviewId, int userId, bool isAdmin, string reply);
    Task<List<ReviewItem>> GetPartnerReviewsAsync(int partnerUserId);
    Task<PagedReviews> GetAdminReviewsAsync(string? search, string? status, int? stars, int? maxStars, int page, int pageSize);
    Task<(ReviewItem? review, string? error)> SetHiddenAsync(int reviewId, bool hidden, string? reason);
}

public class ReviewService : IReviewService
{
    private readonly AppDbContext _db;
    private readonly IInAppNotifier _inApp;
    private readonly IAuditService _audit;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(AppDbContext db, IInAppNotifier inApp, IAuditService audit, ILogger<ReviewService> logger)
    {
        _db = db;
        _inApp = inApp;
        _audit = audit;
        _logger = logger;
    }

    private static IQueryable<ReviewItem> Project(IQueryable<Review> q) => q.Select(r => new ReviewItem(
        r.Id,
        r.RoomId,
        r.Room.Name,
        r.Room.Property.Name,
        r.Room.Property.City,
        r.UserId,
        r.User.FullName,
        r.User.Avatar,
        r.Stars,
        r.Comment,
        r.CreatedAt,
        r.BookingId,
        r.BookingId == null ? null : (int?)r.Room.Bookings.Where(b => b.Id == r.BookingId).Select(b => b.Nights).FirstOrDefault(),
        r.Reply,
        r.ReplyAt,
        r.IsHidden,
        r.HiddenReason));

    public async Task<RoomReviewsResponse> GetRoomReviewsAsync(int roomId, int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);
        var visible = _db.Reviews.Where(r => r.RoomId == roomId && !r.IsHidden);

        var stars = await visible.Select(r => r.Stars).ToListAsync();
        var distribution = new int[5];
        foreach (var s in stars) if (s is >= 1 and <= 5) distribution[s - 1]++;

        var property = await _db.Rooms.Where(r => r.Id == roomId)
            .Select(r => new { r.Property.AverageRating, r.Property.TotalReviews })
            .FirstOrDefaultAsync();

        var items = await Project(visible.OrderByDescending(r => r.CreatedAt).ThenByDescending(r => r.Id)
                .Skip((page - 1) * pageSize).Take(pageSize))
            .ToListAsync();

        return new RoomReviewsResponse(
            roomId,
            stars.Count > 0 ? Math.Round(stars.Average(), 1) : null,
            stars.Count,
            distribution,
            property?.AverageRating is null ? null : (double)property.AverageRating,
            property?.TotalReviews ?? 0,
            page,
            pageSize,
            items);
    }

    public async Task<List<ReviewItem>> GetFeaturedAsync(int take)
    {
        take = Math.Clamp(take, 1, 20);
        // Đánh giá tốt gần đây, có bình luận đủ dài, mỗi phòng một đánh giá, chỉ phòng đang mở bán
        var candidates = await Project(_db.Reviews
                .Where(r => !r.IsHidden && r.Stars >= 4 && r.Comment != null && r.Comment.Length >= 30 &&
                            r.Room.IsApproved == true && r.Room.Status != "hidden" && r.Room.Status != "rejected")
                .OrderByDescending(r => r.CreatedAt)
                .Take(take * 5))
            .ToListAsync();
        return candidates.GroupBy(r => r.RoomId).Select(g => g.First()).Take(take).ToList();
    }

    public async Task<(ReviewItem? review, string? error)> CreateAsync(int userId, int bookingId, int stars, string? comment)
    {
        if (stars is < 1 or > 5) return (null, "Số sao phải từ 1 đến 5");
        comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        if (comment is { Length: > 2000 }) return (null, "Bình luận tối đa 2000 ký tự");

        var booking = await _db.Bookings
            .Where(b => b.Id == bookingId)
            .Select(b => new { b.Id, b.UserId, b.RoomId, b.Status, b.Room.PropertyId, RoomName = b.Room.Name })
            .FirstOrDefaultAsync();
        if (booking is null) return (null, "Không tìm thấy đơn đặt phòng");
        if (booking.UserId != userId) return (null, "Bạn chỉ đánh giá được đơn của mình");
        if (!BookingStatus.Is(booking.Status, BookingStatus.Completed))
            return (null, "Chỉ đánh giá được sau khi đã trả phòng");
        if (await _db.Reviews.AnyAsync(r => r.BookingId == bookingId))
            return (null, "Bạn đã đánh giá đơn này rồi");

        var review = new Review
        {
            UserId = userId,
            RoomId = booking.RoomId,
            BookingId = bookingId,
            Stars = stars,
            Comment = comment,
            CreatedAt = DateTime.Now,
        };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();
        await RecomputePropertyAsync(booking.PropertyId);

        await _audit.LogAsync("REVIEW_CREATE", "danh_gia", review.Id, null, new { bookingId, stars }, userId);
        await _inApp.NotifyRoomOwnerAsync(booking.RoomId, $"Đánh giá mới {stars}★ cho {booking.RoomName}",
            comment is null ? "Khách không để lại bình luận." : Snippet(comment), NotificationType.Review, "/business/reviews");
        _logger.LogInformation("Review {ReviewId} created for booking {BookingId}", review.Id, bookingId);
        return (await Project(_db.Reviews.Where(r => r.Id == review.Id)).FirstAsync(), null);
    }

    public async Task<(ReviewItem? review, string? error)> ReplyAsync(int reviewId, int userId, bool isAdmin, string reply)
    {
        reply = reply?.Trim() ?? "";
        if (reply.Length is 0) return (null, "Nội dung trả lời không được để trống");
        if (reply.Length > 1000) return (null, "Trả lời tối đa 1000 ký tự");

        var review = await _db.Reviews.Include(r => r.Room).ThenInclude(r => r.Property).ThenInclude(p => p.Partner)
            .FirstOrDefaultAsync(r => r.Id == reviewId);
        if (review is null) return (null, "Không tìm thấy đánh giá");
        if (!isAdmin && review.Room.Property.Partner?.UserId != userId)
            return (null, "Bạn chỉ trả lời được đánh giá về phòng của mình");

        var hadReply = review.Reply is not null;
        review.Reply = reply;
        review.ReplyAt = DateTime.Now;
        await _db.SaveChangesAsync();

        await _audit.LogAsync("REVIEW_REPLY", "danh_gia", reviewId, null, new { reply = Snippet(reply) }, userId);
        if (!hadReply)
            await _inApp.NotifyAsync(review.UserId, $"Chủ nhà đã trả lời đánh giá của bạn",
                Snippet(reply), NotificationType.Review, $"/rooms/{review.RoomId}#reviews");
        return (await Project(_db.Reviews.Where(r => r.Id == reviewId)).FirstAsync(), null);
    }

    public async Task<List<ReviewItem>> GetPartnerReviewsAsync(int partnerUserId) =>
        await Project(_db.Reviews
                .Where(r => r.Room.Property.Partner.UserId == partnerUserId)
                .OrderByDescending(r => r.CreatedAt))
            .ToListAsync();

    public async Task<PagedReviews> GetAdminReviewsAsync(string? search, string? status, int? stars, int? maxStars, int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var q = _db.Reviews.AsQueryable();
        if (status == "hidden") q = q.Where(r => r.IsHidden);
        else if (status == "visible") q = q.Where(r => !r.IsHidden);
        if (stars is >= 1 and <= 5) q = q.Where(r => r.Stars == stars);
        if (maxStars is >= 1 and <= 5) q = q.Where(r => r.Stars <= maxStars);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(r => (r.Comment != null && r.Comment.Contains(s)) || r.User.FullName.Contains(s) ||
                             r.Room.Name.Contains(s) || r.Room.Property.Name.Contains(s));
        }
        var total = await q.CountAsync();
        var items = await Project(q.OrderByDescending(r => r.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize)).ToListAsync();
        return new PagedReviews(total, page, pageSize, items);
    }

    public async Task<(ReviewItem? review, string? error)> SetHiddenAsync(int reviewId, bool hidden, string? reason)
    {
        var review = await _db.Reviews.Include(r => r.Room).FirstOrDefaultAsync(r => r.Id == reviewId);
        if (review is null) return (null, "Không tìm thấy đánh giá");
        reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        if (hidden && reason is null) return (null, "Cần ghi lý do ẩn đánh giá");
        if (reason is { Length: > 500 }) return (null, "Lý do tối đa 500 ký tự");

        review.IsHidden = hidden;
        review.HiddenReason = hidden ? reason : null;
        await _db.SaveChangesAsync();
        await RecomputePropertyAsync(review.Room.PropertyId);
        await _audit.LogAsync(hidden ? "REVIEW_HIDE" : "REVIEW_SHOW", "danh_gia", reviewId, null, new { reason });
        if (hidden)
            await _inApp.NotifyAsync(review.UserId, "Một đánh giá của bạn đã bị ẩn",
                $"Lý do: {reason}", NotificationType.Review, $"/rooms/{review.RoomId}");
        return (await Project(_db.Reviews.Where(r => r.Id == reviewId)).FirstAsync(), null);
    }

    /// <summary>Điểm trung bình của cơ sở chỉ tính đánh giá đang hiển thị.</summary>
    private async Task RecomputePropertyAsync(int propertyId)
    {
        var property = await _db.Properties.FirstOrDefaultAsync(p => p.Id == propertyId);
        if (property is null) return;
        var stars = await _db.Reviews.Where(r => r.Room.PropertyId == propertyId && !r.IsHidden).Select(r => r.Stars).ToListAsync();
        property.TotalReviews = stars.Count;
        property.AverageRating = stars.Count > 0 ? Math.Round((decimal)stars.Average(), 2) : null;
        await _db.SaveChangesAsync();
    }

    private static string Snippet(string text) => text.Length <= 160 ? text : text[..157] + "…";
}
