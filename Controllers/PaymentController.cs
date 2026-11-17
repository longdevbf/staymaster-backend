using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HotelManagement.API.Common;
using HotelManagement.API.DTOs.Payment;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private static readonly Regex BookingIdRegex = new(@"BK(\d+)", RegexOptions.IgnoreCase);

    private readonly IBookingService _bookingService;
    private readonly IConfiguration _config;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IBookingService bookingService,
        IConfiguration config,
        ILogger<PaymentController> logger)
    {
        _bookingService = bookingService;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Tài khoản nhận tiền để frontend tạo mã VietQR. Lấy từ cấu hình SePay để luôn khớp với tài khoản
    /// mà webhook SePay theo dõi (trước đây frontend viết cứng một tài khoản khác).
    /// </summary>
    [HttpGet("bank-info")]
    public IActionResult BankInfo()
    {
        var bankId = _config["SePay:BankId"];
        var accountNumber = _config["SePay:AccountNumber"];
        if (string.IsNullOrWhiteSpace(bankId) || string.IsNullOrWhiteSpace(accountNumber))
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Chưa cấu hình tài khoản nhận tiền" });

        return Ok(new { bankId, accountNumber, accountName = _config["SePay:AccountName"] ?? "" });
    }

    /// <summary>SePay gọi vào đây khi tài khoản nhận được chuyển khoản.</summary>
    [HttpPost("sepay-webhook")]
    public async Task<IActionResult> SePayWebhook([FromBody] SePayWebhookRequest request)
    {
        // 1. Xác thực: SePay gửi header "Authorization: Apikey <KEY>" (hoặc "Bearer <KEY>")
        var apiKey = _config["SePay:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("SePay webhook rejected: SePay:ApiKey is not configured");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Webhook is not configured" });
        }

        var authHeader = Request.Headers.Authorization.ToString();
        var presentedKey = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "";
        if (!FixedTimeEquals(presentedKey, apiKey))
        {
            _logger.LogWarning("Unauthorized SePay webhook attempt from {Ip}", HttpContext.Connection.RemoteIpAddress);
            return Unauthorized(new { message = "Invalid API Key" });
        }

        _logger.LogInformation("SePay webhook: content {Content}, amount {Amount}", request.BookingContent, request.AmountIn);

        if (!string.Equals(request.transferType, "in", StringComparison.OrdinalIgnoreCase) && request.transferType is not null)
            return Ok(new { message = "Ignored outgoing transfer" });

        // 2. Lấy mã đơn từ nội dung chuyển khoản (BK123)
        var match = BookingIdRegex.Match(request.BookingContent);
        if (!match.Success || !int.TryParse(match.Groups[1].Value, out var bookingId))
        {
            _logger.LogWarning("SePay webhook content contains no booking id: {Content}", request.BookingContent);
            return Ok(new { message = "No booking ID found in content" });
        }

        // 3. Kiểm tra số tiền trước khi xác nhận
        var info = await _bookingService.GetAccessInfoAsync(bookingId);
        if (info is null || !BookingStatus.Is(info.Status, BookingStatus.Pending))
        {
            _logger.LogWarning("SePay webhook: booking {BookingId} not found or not pending", bookingId);
            return Ok(new { message = "Booking not found or not pending" });
        }

        if (request.AmountIn < info.AmountDue)
        {
            _logger.LogWarning("SePay webhook: booking {BookingId} underpaid ({Paid} < {Due})", bookingId, request.AmountIn, info.AmountDue);
            return Ok(new { message = "Amount is less than the amount due" });
        }

        var result = await _bookingService.ConfirmBookingAsync(bookingId);
        if (result == null)
            return Ok(new { message = "Booking could not be confirmed" });

        _logger.LogInformation("Booking {BookingId} confirmed via SePay webhook", bookingId);
        return Ok(new { success = true, bookingId });
    }

    private static bool FixedTimeEquals(string a, string b) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
}
