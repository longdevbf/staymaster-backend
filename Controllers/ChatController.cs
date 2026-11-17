using HotelManagement.API.Common;
using HotelManagement.API.Services.Ai;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chat;
    public ChatController(IChatService chat) => _chat = chat;

    // POST /api/chat — không bắt buộc đăng nhập; cần đăng nhập để chuyển câu hỏi cho chủ nhà / admin
    [HttpPost]
    [EnableRateLimiting("chat")]
    public async Task<ActionResult<ChatReply>> Chat([FromBody] ChatRequest request, CancellationToken ct)
    {
        if (request.Messages.Count == 0)
            return BadRequest(new { message = "Thiếu nội dung tin nhắn" });

        return Ok(await _chat.ReplyAsync(request, User.GetUserId(), ct));
    }
}
