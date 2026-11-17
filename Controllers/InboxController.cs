using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.Models;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Controllers;

public class AskQuestionRequest
{
    public int RoomId { get; set; }
    public string Question { get; set; } = string.Empty;
}

public class AnswerQuestionRequest
{
    public string Answer { get; set; } = string.Empty;
}

public class SendMessageRequest
{
    public string Content { get; set; } = string.Empty;
}

public class InboxMessageDto
{
    public int Id { get; set; }
    /// <summary>"customer", "host" hoặc "support".</summary>
    public string Sender { get; set; } = InboxSender.Customer;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class InboxQuestionDto
{
    public int Id { get; set; }
    /// <summary>Chỉ hiện tên khách; email/số điện thoại không lộ ra cho Đối tác (SRS 3c).</summary>
    public string CustomerName { get; set; } = string.Empty;
    /// <summary>Tên cơ sở trả lời (hiện cho khách).</summary>
    public string? HostName { get; set; }
    public int? RoomId { get; set; }
    public string? RoomName { get; set; }
    public string Question { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public string Source { get; set; } = "direct";
    /// <summary>"open" = đang chờ chủ nhà/hỗ trợ; "answered" = tin cuối là của chủ nhà/hỗ trợ.</summary>
    public string Status { get; set; } = "open";
    /// <summary>"partner" hoặc "admin".</summary>
    public string Target { get; set; } = "partner";
    public DateTime CreatedAt { get; set; }
    public DateTime? AnsweredAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public List<InboxMessageDto> Messages { get; set; } = new();
}

/// <summary>
/// Hòm thư (SRS 2e): mỗi câu hỏi là một cuộc trò chuyện giữa khách và chủ nhà (hoặc bộ phận hỗ trợ).
/// Chủ nhà/hỗ trợ trả lời thì khách thấy ngay trong khung chat và nhận email.
/// </summary>
[ApiController]
[Route("api/inbox")]
[Authorize]
public class InboxController : ControllerBase
{
    private const int MaxMessageLength = 2000;

    private readonly AppDbContext _db;
    private readonly IInboxService _inbox;

    public InboxController(AppDbContext db, IInboxService inbox)
    {
        _db = db;
        _inbox = inbox;
    }

    // POST /api/inbox — khách hỏi chủ nhà của một phòng
    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] AskQuestionRequest req)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var text = req.Question?.Trim() ?? "";
        if (text.Length is 0 or > MaxMessageLength)
            return BadRequest(new { message = $"Câu hỏi cần từ 1 đến {MaxMessageLength} ký tự" });

        var partnerId = await _db.Rooms
            .Where(r => r.Id == req.RoomId && r.IsApproved == true && r.Status != "deleted")
            .Select(r => (int?)r.Property.PartnerId)
            .FirstOrDefaultAsync();
        if (partnerId is null) return NotFound(new { message = "Không tìm thấy phòng" });

        var q = await _inbox.CreateAsync(userId.Value, partnerId, req.RoomId, text, "direct");
        return Ok(new { id = q.Id, message = "Đã gửi câu hỏi. Chủ nhà sẽ trả lời trong khung chat, bạn nhận thông báo khi có trả lời." });
    }

    // GET /api/inbox?status=open|answered|all — Đối tác: cuộc trò chuyện về cơ sở của mình; Admin: gửi bộ phận hỗ trợ
    [HttpGet]
    [Authorize(Roles = AppRoles.PartnerOrAdmin)]
    public async Task<IActionResult> List([FromQuery] string status = "open")
    {
        var query = _db.InboxQuestions.AsQueryable();
        if (User.IsAdmin())
        {
            query = query.Where(q => q.PartnerId == null);
        }
        else
        {
            var partnerId = await CurrentPartnerIdAsync();
            if (partnerId is null) return Ok(Array.Empty<InboxQuestionDto>());
            query = query.Where(q => q.PartnerId == partnerId);
        }

        if (status is "open" or "answered") query = query.Where(q => q.Status == status);

        return Ok(await Project(query.OrderByDescending(q => q.LastMessageAt ?? q.CreatedAt).Take(200)).ToListAsync());
    }

    // GET /api/inbox/mine — các cuộc trò chuyện của khách
    [HttpGet("mine")]
    public async Task<IActionResult> Mine()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        return Ok(await Project(_db.InboxQuestions
            .Where(q => q.CustomerUserId == userId)
            .OrderByDescending(q => q.LastMessageAt ?? q.CreatedAt)
            .Take(100)).ToListAsync());
    }

    // GET /api/inbox/{id} — một cuộc trò chuyện (khách của nó, chủ nhà của nó, admin)
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var (q, side, denied) = await AuthorizeAsync(id);
        if (denied is not null) return denied;

        return Ok(await Project(_db.InboxQuestions.Where(x => x.Id == q!.Id)).FirstAsync());
    }

    // POST /api/inbox/{id}/messages — nhắn tiếp trong cuộc trò chuyện
    [HttpPost("{id:int}/messages")]
    public async Task<IActionResult> Send(int id, [FromBody] SendMessageRequest req)
    {
        var text = req.Content?.Trim() ?? "";
        if (text.Length is 0 or > MaxMessageLength)
            return BadRequest(new { message = $"Tin nhắn cần từ 1 đến {MaxMessageLength} ký tự" });

        var (q, side, denied) = await AuthorizeAsync(id);
        if (denied is not null) return denied;

        var m = await _inbox.AddMessageAsync(q!, User.GetUserId()!.Value, side!, text);
        return Ok(new InboxMessageDto { Id = m.Id, Sender = m.Sender, Content = m.Content, CreatedAt = m.CreatedAt });
    }

    // POST /api/inbox/{id}/answer — giữ cho client cũ: chủ nhà/hỗ trợ trả lời
    [HttpPost("{id:int}/answer")]
    [Authorize(Roles = AppRoles.PartnerOrAdmin)]
    public async Task<IActionResult> Answer(int id, [FromBody] AnswerQuestionRequest req)
    {
        var (_, side, denied) = await AuthorizeAsync(id);
        if (denied is not null) return denied;
        if (side == InboxSender.Customer)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Bạn không có quyền trả lời câu hỏi này" });

        var result = await Send(id, new SendMessageRequest { Content = req.Answer });
        return result is OkObjectResult ? Ok(new { message = "Đã gửi câu trả lời cho khách" }) : result;
    }

    /// <summary>Xác định người dùng ở phía nào của cuộc trò chuyện; không thuộc phía nào thì từ chối.</summary>
    private async Task<(InboxQuestion? q, string? side, IActionResult? denied)> AuthorizeAsync(int id)
    {
        var q = await _db.InboxQuestions.FirstOrDefaultAsync(x => x.Id == id);
        if (q is null) return (null, null, NotFound(new { message = "Không tìm thấy cuộc trò chuyện" }));

        var userId = User.GetUserId();
        if (userId == q.CustomerUserId) return (q, InboxSender.Customer, null);
        if (User.IsAdmin()) return (q, InboxSender.Support, null);
        if (User.IsPartner() && q.PartnerId is not null && q.PartnerId == await CurrentPartnerIdAsync())
            return (q, InboxSender.Host, null);

        return (null, null, StatusCode(StatusCodes.Status403Forbidden, new { message = "Bạn không có quyền xem cuộc trò chuyện này" }));
    }

    private IQueryable<InboxQuestionDto> Project(IQueryable<InboxQuestion> query) =>
        query.Select(q => new InboxQuestionDto
        {
            Id = q.Id,
            CustomerName = q.Customer.FullName,
            HostName = q.Partner != null ? q.Partner.BusinessName : "Hỗ trợ StayMaster",
            RoomId = q.RoomId,
            RoomName = q.Room != null ? q.Room.Name : null,
            Question = q.Question,
            Answer = q.Answer,
            Source = q.Source,
            Status = q.Status,
            Target = q.PartnerId == null ? "admin" : "partner",
            CreatedAt = q.CreatedAt,
            AnsweredAt = q.AnsweredAt,
            LastMessageAt = q.LastMessageAt,
            Messages = q.Messages.OrderBy(m => m.CreatedAt).Select(m => new InboxMessageDto
            {
                Id = m.Id,
                Sender = m.Sender,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
            }).ToList(),
        });

    private async Task<int?> CurrentPartnerIdAsync()
    {
        var userId = User.GetUserId();
        if (userId is null) return null;
        var id = await _db.Partners.Where(p => p.UserId == userId).Select(p => p.Id).FirstOrDefaultAsync();
        return id == 0 ? null : id;
    }
}
