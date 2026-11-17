namespace HotelManagement.API.Models;

/// <summary>Một tin nhắn trong cuộc trò chuyện giữa khách và chủ nhà / bộ phận hỗ trợ (bảng hop_thu_tin_nhan).</summary>
public class InboxMessage
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public int? SenderUserId { get; set; }
    /// <summary>"customer", "host" (chủ nhà) hoặc "support" (admin).</summary>
    public string Sender { get; set; } = "customer";
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public InboxQuestion Question { get; set; } = null!;
}
