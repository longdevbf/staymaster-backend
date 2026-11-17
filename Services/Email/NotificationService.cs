using System.Globalization;
using System.Net;
using HotelManagement.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Services.Email;

public enum BookingEvent
{
    Created,
    Confirmed,
    Cancelled,
    Completed,
    /// <summary>Tự huỷ vì quá thời gian giữ phòng mà chưa thanh toán</summary>
    Expired,
}

/// <summary>Soạn email thông báo và đưa vào hàng đợi. Không bao giờ ném lỗi ra ngoài luồng nghiệp vụ.</summary>
public interface INotificationService
{
    Task NotifyBookingAsync(int bookingId, BookingEvent evt, string? reason = null);
    /// <summary>Tiền về cho đơn đã huỷ / không còn chờ: báo admin hoàn tiền thủ công</summary>
    Task NotifyPaymentNeedsRefundAsync(int bookingId, decimal amount);
    void NotifyPasswordReset(string email, string fullName, string resetLink);
    void NotifyInboxAnswer(string email, string fullName, string question, string answer, string answeredBy);
    /// <summary>Báo chủ nhà (hoặc bộ phận hỗ trợ) có khách đang chờ trả lời.</summary>
    void NotifyHostNewMessage(string email, string hostName, string customerName, string? roomName, string message, bool support);
}

public class NotificationService : INotificationService
{
    private static readonly CultureInfo Vi = new("vi-VN");

    private readonly AppDbContext _db;
    private readonly IEmailQueue _queue;
    private readonly IConfiguration _config;
    private readonly ILogger<NotificationService> _logger;
    private readonly IInAppNotifier _inApp;

    public NotificationService(AppDbContext db, IEmailQueue queue, IConfiguration config, ILogger<NotificationService> logger,
        IInAppNotifier inApp)
    {
        _inApp = inApp;
        _db = db;
        _queue = queue;
        _config = config;
        _logger = logger;
    }

    private string FrontendUrl => (_config["App:FrontendUrl"] ?? "http://localhost:8081").TrimEnd('/');

    public async Task NotifyBookingAsync(int bookingId, BookingEvent evt, string? reason = null)
    {
        try
        {
            var b = await _db.Bookings
                .Where(x => x.Id == bookingId)
                .Select(x => new
                {
                    x.Id,
                    x.CheckIn,
                    x.CheckOut,
                    x.Nights,
                    x.TotalPrice,
                    x.ServiceFee,
                    x.Deposit,
                    x.CreatedAt,
                    x.UserId,
                    x.RoomId,
                    PartnerUserId = (int?)x.Room.Property.Partner.UserId,
                    CustomerName = x.User.FullName,
                    CustomerEmail = x.User.Email,
                    RoomName = x.Room.Name,
                    PropertyName = x.Room.Property.Name,
                    Address = x.Room.Property.DetailedAddress + ", " + x.Room.Property.City,
                })
                .FirstOrDefaultAsync();
            if (b is null) return;

            var (subject, intro) = evt switch
            {
                BookingEvent.Created => ($"Đã nhận đơn đặt phòng #{b.Id}",
                    "Chúng tôi đã nhận được đơn đặt phòng của bạn. Đơn sẽ được xác nhận khi thanh toán thành công hoặc khi chủ nhà duyệt."),
                BookingEvent.Confirmed => ($"Đơn đặt phòng #{b.Id} đã được xác nhận",
                    "Đơn đặt phòng của bạn đã được xác nhận. Hẹn gặp bạn vào ngày nhận phòng!"),
                BookingEvent.Cancelled => ($"Đơn đặt phòng #{b.Id} đã bị huỷ",
                    "Đơn đặt phòng dưới đây đã bị huỷ. Nếu bạn không yêu cầu huỷ, vui lòng liên hệ chúng tôi."),
                BookingEvent.Completed => ($"Cảm ơn bạn đã lưu trú — hoá đơn đơn #{b.Id}",
                    "Chuyến đi đã hoàn tất. Bạn có thể tải hoá đơn PDF và viết đánh giá trong mục \"Đơn đặt phòng của tôi\"."),
                BookingEvent.Expired => ($"Đơn đặt phòng #{b.Id} đã tự huỷ",
                    "Đơn chưa được thanh toán trong thời gian giữ phòng nên đã tự huỷ và phòng được mở lại. Nếu bạn vẫn muốn ở, hãy đặt lại; nếu bạn đã chuyển khoản, StayMaster sẽ liên hệ để xử lý."),
                _ => ($"Cập nhật đơn #{b.Id}", ""),
            };

            var rows = new[]
            {
                ("Mã đơn", $"#{b.Id}"),
                ("Phòng", $"{b.RoomName} — {b.PropertyName}"),
                ("Địa chỉ", b.Address),
                ("Nhận phòng", b.CheckIn.ToString("dd/MM/yyyy")),
                ("Trả phòng", b.CheckOut.ToString("dd/MM/yyyy")),
                ("Số đêm", b.Nights.ToString()),
                ("Tổng tiền", Money(b.TotalPrice)),
            };

            var html = Layout(subject, $"""
                <p>Xin chào {Enc(b.CustomerName)},</p>
                <p>{Enc(intro)}</p>
                {(string.IsNullOrWhiteSpace(reason) ? "" : $"<p><b>Lý do:</b> {Enc(reason)}</p>")}
                {Table(rows)}
                <p><a href="{FrontendUrl}/my-bookings" style="color:#135bec">Xem đơn đặt phòng của tôi</a></p>
                """);

            _queue.Enqueue(new EmailMessage(b.CustomerEmail, subject, html));

            // Thông báo trong app cho khách và chủ nhà
            var dates = $"{b.CheckIn:dd/MM} → {b.CheckOut:dd/MM}";
            var reasonText = string.IsNullOrWhiteSpace(reason) ? "" : $" Lý do: {reason}";
            switch (evt)
            {
                case BookingEvent.Created:
                    await _inApp.NotifyAsync(b.UserId, $"Đơn #{b.Id} đang giữ phòng",
                        $"{b.RoomName}, {dates}. Chuyển khoản {Money(b.Deposit ?? b.TotalPrice)} để hoàn tất đặt phòng.",
                        NotificationType.Booking, $"/booking/payment?bookingId={b.Id}");
                    if (b.PartnerUserId is int p1)
                        await _inApp.NotifyAsync(p1, $"Đơn mới #{b.Id} chờ thanh toán",
                            $"{b.CustomerName} đặt {b.RoomName}, {dates}.", NotificationType.Booking, "/business/dashboard");
                    break;
                case BookingEvent.Confirmed:
                    await _inApp.NotifyAsync(b.UserId, $"Đơn #{b.Id} đã được xác nhận",
                        $"{b.RoomName}, {dates}. Hẹn gặp bạn!", NotificationType.Booking, $"/my-bookings/{b.Id}");
                    if (b.PartnerUserId is int p2)
                        await _inApp.NotifyAsync(p2, $"Đơn #{b.Id} đã thanh toán",
                            $"{b.CustomerName} · {b.RoomName}, {dates} · {Money(b.TotalPrice)}.", NotificationType.Booking, "/business/checkin-out");
                    break;
                case BookingEvent.Cancelled:
                    await _inApp.NotifyAsync(b.UserId, $"Đơn #{b.Id} đã huỷ", $"{b.RoomName}, {dates}.{reasonText}",
                        NotificationType.Booking, $"/my-bookings/{b.Id}");
                    if (b.PartnerUserId is int p3)
                        await _inApp.NotifyAsync(p3, $"Đơn #{b.Id} đã huỷ", $"{b.CustomerName} · {b.RoomName}, {dates}.{reasonText}",
                            NotificationType.Booking, "/business/dashboard");
                    break;
                case BookingEvent.Expired:
                    await _inApp.NotifyAsync(b.UserId, $"Đơn #{b.Id} đã tự huỷ",
                        $"Chưa nhận được thanh toán nên phòng {b.RoomName} ({dates}) đã được mở lại.", NotificationType.Booking, $"/rooms/{b.RoomId}");
                    if (b.PartnerUserId is int p4)
                        await _inApp.NotifyAsync(p4, $"Đơn #{b.Id} tự huỷ vì quá hạn thanh toán",
                            $"{b.CustomerName} · {b.RoomName}, {dates}. Phòng đã mở lại.", NotificationType.Booking, "/business/dashboard");
                    break;
                case BookingEvent.Completed:
                    await _inApp.NotifyAsync(b.UserId, "Chuyến đi thế nào?",
                        $"Cảm ơn bạn đã ở {b.RoomName}. Viết vài dòng đánh giá để giúp khách sau nhé.", NotificationType.Review, $"/my-bookings/{b.Id}?review=1");
                    if (b.PartnerUserId is int p5)
                        await _inApp.NotifyAsync(p5, $"Đã cộng {Money(b.TotalPrice - (b.ServiceFee ?? 0))} vào số dư",
                            $"Đơn #{b.Id} của {b.CustomerName} đã trả phòng.", NotificationType.Payout, "/business/payments");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not queue {Event} email for booking {BookingId}", evt, bookingId);
        }
    }

    public async Task NotifyPaymentNeedsRefundAsync(int bookingId, decimal amount)
    {
        await _inApp.NotifyAdminsAsync($"Cần hoàn tiền cho đơn #{bookingId}",
            $"Nhận {Money(amount)} cho đơn đã huỷ hoặc không còn chờ thanh toán. Kiểm tra và hoàn tiền cho khách.",
            NotificationType.Payout, $"/admin/bookings?id={bookingId}");
        var support = _config["App:SupportEmail"];
        if (!string.IsNullOrWhiteSpace(support))
        {
            var subject = $"Cần hoàn tiền cho đơn #{bookingId}";
            _queue.Enqueue(new EmailMessage(support, subject, Layout(subject,
                $"<p>SePay báo đã nhận {Enc(Money(amount))} với nội dung BK{bookingId}, nhưng đơn không còn chờ thanh toán. Vui lòng kiểm tra và hoàn tiền cho khách.</p>")));
        }
    }

    public void NotifyPasswordReset(string email, string fullName, string resetLink)
    {
        const string subject = "Đặt lại mật khẩu StayMaster";
        var html = Layout(subject, $"""
            <p>Xin chào {Enc(fullName)},</p>
            <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Bấm vào nút dưới đây trong vòng 30 phút:</p>
            <p><a href="{Enc(resetLink)}" style="display:inline-block;padding:12px 20px;background:#135bec;color:#fff;border-radius:8px;text-decoration:none;font-weight:bold">Đặt lại mật khẩu</a></p>
            <p style="color:#64748b;font-size:13px">Nếu bạn không yêu cầu, hãy bỏ qua email này — mật khẩu của bạn không thay đổi.</p>
            """);
        _queue.Enqueue(new EmailMessage(email, subject, html));
    }

    public void NotifyInboxAnswer(string email, string fullName, string question, string answer, string answeredBy)
    {
        const string subject = "Phản hồi cho câu hỏi của bạn trên StayMaster";
        var html = Layout(subject, $"""
            <p>Xin chào {Enc(fullName)},</p>
            <p>{Enc(answeredBy)} đã trả lời câu hỏi của bạn:</p>
            <blockquote style="margin:0;padding:8px 12px;border-left:3px solid #cbd5e1;color:#475569">{Enc(question)}</blockquote>
            <p style="white-space:pre-line">{Enc(answer)}</p>
            """);
        _queue.Enqueue(new EmailMessage(email, subject, html));
    }

    public void NotifyHostNewMessage(string email, string hostName, string customerName, string? roomName, string message, bool support)
    {
        var subject = support ? $"Yêu cầu hỗ trợ mới từ {customerName}" : $"Khách {customerName} đang chờ bạn trả lời";
        var inbox = support ? "/admin/inbox" : "/business/inbox";
        var html = Layout(subject, $"""
            <p>Xin chào {Enc(hostName)},</p>
            <p>{Enc(customerName)} vừa nhắn{(roomName is null ? "" : $" về phòng <b>{Enc(roomName)}</b>")}:</p>
            <blockquote style="margin:0;padding:8px 12px;border-left:3px solid #cbd5e1;color:#475569;white-space:pre-line">{Enc(message)}</blockquote>
            <p><a href="{FrontendUrl}{inbox}" style="display:inline-block;padding:10px 18px;background:#135bec;color:#fff;border-radius:8px;text-decoration:none;font-weight:bold">Trả lời trong hòm thư</a></p>
            <p style="color:#64748b;font-size:13px">Khách thấy câu trả lời ngay trong khung chat trên StayMaster.</p>
            """);
        _queue.Enqueue(new EmailMessage(email, subject, html));
    }

    private static string Money(decimal value) => value.ToString("#,##0", Vi) + " đ";

    private static string Enc(string? value) => WebUtility.HtmlEncode(value ?? "");

    private static string Table(IEnumerable<(string Label, string Value)> rows) =>
        "<table style=\"border-collapse:collapse;width:100%;margin:16px 0\">" +
        string.Concat(rows.Select(r =>
            $"<tr><td style=\"padding:6px 8px;color:#64748b;border-bottom:1px solid #e2e8f0\">{Enc(r.Label)}</td>" +
            $"<td style=\"padding:6px 8px;font-weight:bold;border-bottom:1px solid #e2e8f0\">{Enc(r.Value)}</td></tr>")) +
        "</table>";

    private static string Layout(string title, string body) => $"""
        <!doctype html>
        <html lang="vi"><body style="margin:0;background:#f6f6f8;font-family:Arial,Helvetica,sans-serif;color:#0f172a">
        <div style="max-width:560px;margin:24px auto;background:#fff;border-radius:12px;padding:24px;border:1px solid #e2e8f0">
          <h2 style="margin:0 0 16px;font-size:20px">{Enc(title)}</h2>
          {body}
          <p style="margin-top:24px;color:#94a3b8;font-size:12px">StayMaster — email tự động, vui lòng không trả lời.</p>
        </div>
        </body></html>
        """;
}
