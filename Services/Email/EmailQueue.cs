using System.Net;
using System.Net.Mail;
using System.Threading.Channels;

namespace HotelManagement.API.Services.Email;

public record EmailMessage(string To, string Subject, string HtmlBody);

/// <summary>Đưa email vào hàng đợi; gửi thật diễn ra ở <see cref="EmailBackgroundSender"/> để request không phải chờ SMTP.</summary>
public interface IEmailQueue
{
    void Enqueue(EmailMessage message);
}

public class EmailQueue : IEmailQueue
{
    private readonly Channel<EmailMessage> _channel = Channel.CreateBounded<EmailMessage>(
        new BoundedChannelOptions(1000) { FullMode = BoundedChannelFullMode.DropOldest });

    public ChannelReader<EmailMessage> Reader => _channel.Reader;

    public void Enqueue(EmailMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.To)) return;
        _channel.Writer.TryWrite(message);
    }
}

public class EmailOptions
{
    public string? SmtpHost { get; set; }
    public int SmtpPort { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? FromAddress { get; set; }
    public string FromName { get; set; } = "StayMaster";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(SmtpHost) && !string.IsNullOrWhiteSpace(FromAddress);
}

/// <summary>
/// Gửi email trong nền. Chưa cấu hình SMTP thì chỉ ghi log (ở Development ghi cả nội dung để thử link).
/// Mục tiêu SRS 3a: email xác nhận đi trong vòng 30 giây sau khi tạo đơn.
/// </summary>
public class EmailBackgroundSender : BackgroundService
{
    private readonly EmailQueue _queue;
    private readonly EmailOptions _options;
    private readonly ILogger<EmailBackgroundSender> _logger;
    private readonly IHostEnvironment _env;

    public EmailBackgroundSender(EmailQueue queue, Microsoft.Extensions.Options.IOptions<EmailOptions> options,
        ILogger<EmailBackgroundSender> logger, IHostEnvironment env)
    {
        _queue = queue;
        _options = options.Value;
        _logger = logger;
        _env = env;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.IsConfigured)
            _logger.LogWarning("Email:SmtpHost / Email:FromAddress not configured — emails are logged, not sent");

        await foreach (var message in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            if (!_options.IsConfigured)
            {
                if (_env.IsDevelopment())
                    _logger.LogInformation("[email not sent] To {To} | {Subject}\n{Body}", message.To, message.Subject, message.HtmlBody);
                else
                    _logger.LogInformation("[email not sent] To {To} | {Subject}", message.To, message.Subject);
                continue;
            }

            await SendWithRetryAsync(message, stoppingToken);
        }
    }

    private async Task SendWithRetryAsync(EmailMessage message, CancellationToken ct)
    {
        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
                {
                    EnableSsl = _options.UseSsl,
                    Credentials = string.IsNullOrEmpty(_options.Username)
                        ? null
                        : new NetworkCredential(_options.Username, _options.Password),
                    Timeout = 15000,
                };
                using var mail = new MailMessage
                {
                    From = new MailAddress(_options.FromAddress!, _options.FromName),
                    Subject = message.Subject,
                    Body = message.HtmlBody,
                    IsBodyHtml = true,
                };
                mail.To.Add(message.To);
                await client.SendMailAsync(mail, ct);
                _logger.LogInformation("Email sent to {To}: {Subject}", message.To, message.Subject);
                return;
            }
            catch (Exception ex) when (attempt < 3 && !ct.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "Email to {To} failed (attempt {Attempt}), retrying", message.To, attempt);
                await Task.Delay(TimeSpan.FromSeconds(2 * attempt), ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email to {To} failed: {Subject}", message.To, message.Subject);
                return;
            }
        }
    }
}
