namespace HotelManagement.API.Models;

/// <summary>
/// Câu hỏi khách gửi cho Đối tác hoặc Admin (bảng hop_thu_cau_hoi), từ chatbot chuyển tiếp hoặc hỏi trực tiếp.
/// PartnerId null = gửi Admin.
/// </summary>
public class InboxQuestion
{
    public int Id { get; set; }
    public int CustomerUserId { get; set; }
    public int? PartnerId { get; set; }
    public int? RoomId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public int? AnsweredByUserId { get; set; }
    /// <summary>"chatbot" hoặc "direct".</summary>
    public string Source { get; set; } = "direct";
    /// <summary>"open" = đang chờ chủ nhà/hỗ trợ trả lời; "answered" = tin nhắn cuối là của chủ nhà/hỗ trợ.</summary>
    public string Status { get; set; } = "open";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? AnsweredAt { get; set; }
    /// <summary>Thời điểm tin nhắn gần nhất, để sắp xếp hòm thư.</summary>
    public DateTime? LastMessageAt { get; set; }

    public ICollection<InboxMessage> Messages { get; set; } = new List<InboxMessage>();

    public User Customer { get; set; } = null!;
    public Partner? Partner { get; set; }
    public Room? Room { get; set; }
}
