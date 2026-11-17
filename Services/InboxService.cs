using HotelManagement.API.Data;
using HotelManagement.API.Models;
using HotelManagement.API.Services.Email;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Services;

public static class InboxSender
{
    public const string Customer = "customer";
    public const string Host = "host";
    public const string Support = "support";
}

/// <summary>Cuộc trò chuyện giữa khách và chủ nhà / bộ phận hỗ trợ (SRS 2e).</summary>
public interface IInboxService
{
    /// <summary>Mở cuộc trò chuyện mới với tin nhắn đầu tiên của khách. partnerId null = gửi bộ phận hỗ trợ.</summary>
    Task<InboxQuestion> CreateAsync(int customerUserId, int? partnerId, int? roomId, string text, string source, CancellationToken ct = default);

    /// <summary>Thêm tin nhắn và cập nhật trạng thái; chủ nhà/hỗ trợ trả lời thì gửi email báo khách.</summary>
    Task<InboxMessage> AddMessageAsync(InboxQuestion question, int senderUserId, string sender, string content, CancellationToken ct = default);
}

public class InboxService : IInboxService
{
    private readonly AppDbContext _db;
    private readonly INotificationService _notifications;
    private readonly ILogger<InboxService> _logger;
    private readonly IConfiguration _config;
    private readonly IInAppNotifier _inApp;

    public InboxService(AppDbContext db, INotificationService notifications, ILogger<InboxService> logger, IConfiguration config,
        IInAppNotifier inApp)
    {
        _inApp = inApp;
        _db = db;
        _notifications = notifications;
        _logger = logger;
        _config = config;
    }

    public async Task<InboxQuestion> CreateAsync(int customerUserId, int? partnerId, int? roomId, string text, string source, CancellationToken ct = default)
    {
        var now = DateTime.Now;
        var q = new InboxQuestion
        {
            CustomerUserId = customerUserId,
            PartnerId = partnerId,
            RoomId = roomId,
            Question = text,
            Source = source,
            Status = "open",
            CreatedAt = now,
            LastMessageAt = now,
        };
        q.Messages.Add(new InboxMessage { SenderUserId = customerUserId, Sender = InboxSender.Customer, Content = text, CreatedAt = now });
        _db.InboxQuestions.Add(q);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Conversation {QuestionId} opened ({Source}) for {Target}", q.Id, source,
            partnerId is null ? "support" : $"partner {partnerId}");
        await NotifyHostAsync(q.Id, text, ct);
        return q;
    }

    public async Task<InboxMessage> AddMessageAsync(InboxQuestion question, int senderUserId, string sender, string content, CancellationToken ct = default)
    {
        var now = DateTime.Now;
        var wasWaiting = question.Status == "open";
        var message = new InboxMessage { QuestionId = question.Id, SenderUserId = senderUserId, Sender = sender, Content = content, CreatedAt = now };
        _db.InboxMessages.Add(message);

        question.LastMessageAt = now;
        if (sender == InboxSender.Customer)
        {
            question.Status = "open";
        }
        else
        {
            question.Status = "answered";
            question.Answer = content; // giữ trường cũ = câu trả lời mới nhất
            question.AnsweredAt = now;
            question.AnsweredByUserId = senderUserId;
        }
        await _db.SaveChangesAsync(ct);

        // Báo chủ nhà/hỗ trợ khi cuộc trò chuyện chuyển sang chờ họ trả lời (không gửi cho từng tin liên tiếp của khách)
        if (sender == InboxSender.Customer && !wasWaiting)
            await NotifyHostAsync(question.Id, content, ct);

        // Báo khách qua email khi chủ nhà/hỗ trợ trả lời một tin nhắn đang chờ (không gửi cho từng tin liên tiếp)
        if (sender != InboxSender.Customer && wasWaiting)
        {
            var info = await _db.InboxQuestions
                .Where(q => q.Id == question.Id)
                .Select(q => new { q.Customer.Email, q.Customer.FullName, BusinessName = q.Partner != null ? q.Partner.BusinessName : null })
                .FirstAsync(ct);
            var lastCustomerMessage = await _db.InboxMessages
                .Where(m => m.QuestionId == question.Id && m.Sender == InboxSender.Customer)
                .OrderByDescending(m => m.CreatedAt).Select(m => m.Content).FirstOrDefaultAsync(ct) ?? question.Question;
            var answeredBy = sender == InboxSender.Host && !string.IsNullOrWhiteSpace(info.BusinessName)
                ? $"Chủ nhà {info.BusinessName}" : "Bộ phận hỗ trợ StayMaster";
            _notifications.NotifyInboxAnswer(info.Email, info.FullName, lastCustomerMessage, content, answeredBy);
        }

        // Thông báo trong app: khách nhận khi chủ nhà/hỗ trợ trả lời (mở thẳng cuộc trò chuyện)
        if (sender != InboxSender.Customer && wasWaiting)
            await _inApp.NotifyAsync(question.CustomerUserId, sender == InboxSender.Host ? "Chủ nhà đã trả lời bạn" : "StayMaster đã trả lời bạn",
                Snippet(content), NotificationType.Inbox, $"chat:{question.Id}");

        _logger.LogInformation("Conversation {QuestionId}: new {Sender} message", question.Id, sender);
        return message;
    }

    private async Task NotifyHostAsync(int questionId, string message, CancellationToken ct)
    {
        var info = await _db.InboxQuestions
            .Where(q => q.Id == questionId)
            .Select(q => new
            {
                HostUserId = q.Partner != null ? (int?)q.Partner.UserId : null,
                CustomerName = q.Customer.FullName,
                RoomName = q.Room != null ? q.Room.Name : null,
                IsSupport = q.Partner == null,
                HostEmail = q.Partner != null && q.Partner.User != null ? q.Partner.User.Email : null,
                HostName = q.Partner != null ? q.Partner.BusinessName : null,
            })
            .FirstOrDefaultAsync(ct);
        if (info is null) return;

        var title = $"{info.CustomerName} đang chờ trả lời";
        var body = (info.RoomName is null ? "" : $"Về {info.RoomName}: ") + Snippet(message);
        if (info.IsSupport)
            await _inApp.NotifyAdminsAsync(title, body, NotificationType.Inbox, $"/admin/inbox?c={questionId}");
        else if (info.HostUserId is int hostId)
            await _inApp.NotifyAsync(hostId, title, body, NotificationType.Inbox, $"/business/inbox?c={questionId}");

        var email = info.IsSupport ? _config["App:SupportEmail"] : info.HostEmail;
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogInformation("Conversation {QuestionId}: no e-mail address for the {Side}, skipping notification",
                questionId, info.IsSupport ? "support team (App:SupportEmail)" : "host");
            return;
        }
        _notifications.NotifyHostNewMessage(email, info.HostName ?? "bộ phận hỗ trợ", info.CustomerName, info.RoomName, message, info.IsSupport);
    }

    private static string Snippet(string text) => text.Length <= 160 ? text : text[..157] + "…";
}
