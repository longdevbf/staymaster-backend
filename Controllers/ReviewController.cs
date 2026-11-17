using System.ComponentModel.DataAnnotations;
using HotelManagement.API.Common;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Controllers;

public class CreateReviewRequest
{
    [Range(1, 5)] public int Stars { get; set; }
    [MaxLength(2000)] public string? Comment { get; set; }
}

public class ReviewReplyRequest
{
    [Required, MaxLength(1000)] public string Reply { get; set; } = string.Empty;
}

public class HideReviewRequest
{
    [MaxLength(500)] public string? Reason { get; set; }
}

/// <summary>Đánh giá phòng: khách viết sau khi trả phòng, chủ nhà trả lời, admin ẩn đánh giá vi phạm.</summary>
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviews;

    public ReviewController(IReviewService reviews) => _reviews = reviews;

    // GET /api/rooms/{roomId}/reviews?page=1&pageSize=10
    [HttpGet("api/rooms/{roomId:int}/reviews")]
    [AllowAnonymous]
    public async Task<IActionResult> RoomReviews(int roomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        Ok(await _reviews.GetRoomReviewsAsync(roomId, page, pageSize));

    // GET /api/reviews/featured?take=6  — đánh giá nổi bật cho trang chủ
    [HttpGet("api/reviews/featured")]
    [AllowAnonymous]
    public async Task<IActionResult> Featured([FromQuery] int take = 6) => Ok(await _reviews.GetFeaturedAsync(take));

    // POST /api/bookings/{bookingId}/review  — khách đánh giá đơn đã hoàn thành
    [HttpPost("api/bookings/{bookingId:int}/review")]
    [Authorize]
    public async Task<IActionResult> Create(int bookingId, [FromBody] CreateReviewRequest req)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        var (review, error) = await _reviews.CreateAsync(userId.Value, bookingId, req.Stars, req.Comment);
        return error is not null ? BadRequest(new { message = error }) : Ok(review);
    }

    // GET /api/reviews/partner  — đánh giá về các phòng của đối tác
    [HttpGet("api/reviews/partner")]
    [Authorize(Roles = AppRoles.Partner)]
    public async Task<IActionResult> Partner()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        return Ok(await _reviews.GetPartnerReviewsAsync(userId.Value));
    }

    // PUT /api/reviews/{id}/reply  — chủ nhà (hoặc admin) trả lời công khai
    [HttpPut("api/reviews/{id:int}/reply")]
    [Authorize(Roles = AppRoles.PartnerOrAdmin)]
    public async Task<IActionResult> Reply(int id, [FromBody] ReviewReplyRequest req)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        var (review, error) = await _reviews.ReplyAsync(id, userId.Value, User.IsAdmin(), req.Reply);
        return error switch
        {
            null => Ok(review),
            "Không tìm thấy đánh giá" => NotFound(new { message = error }),
            "Bạn chỉ trả lời được đánh giá về phòng của mình" => StatusCode(StatusCodes.Status403Forbidden, new { message = error }),
            _ => BadRequest(new { message = error }),
        };
    }

    // GET /api/admin/reviews?search=&status=all|visible|hidden&stars=&maxStars=&page=&pageSize=
    [HttpGet("api/admin/reviews")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> AdminList([FromQuery] string? search, [FromQuery] string? status, [FromQuery] int? stars,
        [FromQuery] int? maxStars, [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
        Ok(await _reviews.GetAdminReviewsAsync(search, status, stars, maxStars, page, pageSize));

    // PUT /api/admin/reviews/{id}/hide  { reason }
    [HttpPut("api/admin/reviews/{id:int}/hide")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Hide(int id, [FromBody] HideReviewRequest req)
    {
        var (review, error) = await _reviews.SetHiddenAsync(id, true, req.Reason);
        return error is not null ? BadRequest(new { message = error }) : Ok(review);
    }

    // PUT /api/admin/reviews/{id}/show
    [HttpPut("api/admin/reviews/{id:int}/show")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Show(int id)
    {
        var (review, error) = await _reviews.SetHiddenAsync(id, false, null);
        return error is not null ? BadRequest(new { message = error }) : Ok(review);
    }
}
