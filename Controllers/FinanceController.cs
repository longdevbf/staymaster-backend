using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.DTOs.Finance;
using HotelManagement.API.Models;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinanceController : ControllerBase
{
    private static readonly string[] PayoutDecisions = { "paid", "rejected" };

    private readonly AppDbContext _context;
    private readonly ILogger<FinanceController> _logger;
    private readonly IAuditService _audit;
    private readonly IInAppNotifier _inApp;

    public FinanceController(AppDbContext context, ILogger<FinanceController> logger, IAuditService audit, IInAppNotifier inApp)
    {
        _inApp = inApp;
        _context = context;
        _logger = logger;
        _audit = audit;
    }

    /// <summary>
    /// Số dư của Đối tác. Doanh thu chỉ được cộng khi chuyến đi hoàn tất (SRS 2d);
    /// đơn đã xác nhận / đang lưu trú hiện ở "UpcomingEarnings".
    /// </summary>
    [HttpGet("business")]
    [Authorize(Roles = AppRoles.Partner)]
    public async Task<IActionResult> GetBusinessSummary()
    {
        var partnerId = await GetCurrentPartnerIdAsync();
        if (partnerId is null) return Ok(new BusinessFinanceSummary());

        var bookings = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .Where(b => b.Room.Property.PartnerId == partnerId &&
                        b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.CancelledVi)
            .ToListAsync();

        var (completedEarnings, pendingPayouts, availableBalance) = await GetBalanceAsync(partnerId.Value);

        var upcoming = bookings
            .Where(b => BookingStatus.Is(b.Status, BookingStatus.Confirmed) || BookingStatus.Is(b.Status, BookingStatus.CheckedIn))
            .Sum(Pricing.PartnerEarning);

        var summary = new BusinessFinanceSummary
        {
            TotalEarnings = completedEarnings,
            UpcomingEarnings = upcoming,
            PendingPayouts = pendingPayouts,
            AvailableBalance = availableBalance,
            RecentTransactions = bookings.OrderByDescending(b => b.CreatedAt)
                .Take(10)
                .Select(b => new FinanceTransactionDto
                {
                    Id = b.Id,
                    RoomName = b.Room?.Name ?? "Phòng trống",
                    CustomerName = b.User?.FullName ?? "Ẩn danh",
                    Amount = Pricing.PartnerEarning(b),
                    Date = b.CreatedAt ?? DateTime.Now,
                    Status = BookingStatus.Normalize(b.Status)
                }).ToList()
        };

        return Ok(summary);
    }

    [HttpGet("admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAdminSummary()
    {
        var totalRevenue = await _context.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.CancelledVi)
            .SumAsync(b => (decimal?)b.TotalPrice) ?? 0;

        // Phần StayMaster giữ lại: phí dịch vụ của các đơn đã hoàn tất
        var platformFees = await _context.Bookings
            .Where(b => b.Status == BookingStatus.Completed)
            .SumAsync(b => (decimal?)b.ServiceFee) ?? 0;

        var totalPayouts = await _context.Payouts
            .Where(p => p.Status == "paid")
            .SumAsync(p => (decimal?)p.Amount) ?? 0;

        var recentBookings = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAt)
            .Take(20)
            .ToListAsync();

        var summary = new AdminFinanceSummary
        {
            TotalRevenue = totalRevenue,
            TotalPayouts = totalPayouts,
            PlatformFees = platformFees,
            TotalBookings = await _context.Bookings.CountAsync(),
            TotalUsers = await _context.Users.CountAsync(),
            TotalProperties = await _context.Properties.CountAsync(),
            SystemTransactions = recentBookings.Select(b => new FinanceTransactionDto
            {
                Id = b.Id,
                RoomName = b.Room?.Name ?? "N/A",
                CustomerName = b.User?.FullName ?? "Unknown",
                Amount = b.TotalPrice,
                Date = b.CreatedAt ?? DateTime.Now,
                Status = BookingStatus.Normalize(b.Status)
            }).ToList()
        };

        return Ok(summary);
    }

    [HttpPost("payout/request")]
    [Authorize(Roles = AppRoles.Partner)]
    public async Task<IActionResult> CreatePayoutRequest(CreatePayoutRequestDto request)
    {
        var partnerId = await GetCurrentPartnerIdAsync();
        if (partnerId is null) return BadRequest(new { message = "Đối tác không tồn tại." });

        if (request.Amount <= 0)
            return BadRequest(new { message = "Số tiền rút phải lớn hơn 0." });

        var (_, _, availableBalance) = await GetBalanceAsync(partnerId.Value);
        if (request.Amount > availableBalance)
            return BadRequest(new { message = $"Số dư không đủ. Số dư khả dụng hiện tại là: {availableBalance:N0} VNĐ" });

        var payout = new Payout
        {
            PartnerId = partnerId.Value,
            Amount = request.Amount,
            BankName = request.BankName,
            AccountNumber = request.AccountNumber,
            AccountHolder = request.AccountHolder,
            Status = "pending",
            CreatedAt = DateTime.Now
        };

        _context.Payouts.Add(payout);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Partner {PartnerId} requested payout {PayoutId} of {Amount}", partnerId, payout.Id, payout.Amount);
        await _audit.LogAsync("PAYOUT_REQUEST", "thanh_toan_doi_tac", payout.Id, null,
            new { payout.Amount, payout.BankName, payout.AccountNumber, availableBalance });
        var businessName = await _context.Partners.Where(p => p.Id == partnerId).Select(p => p.BusinessName).FirstOrDefaultAsync();
        await _inApp.NotifyAdminsAsync($"Yêu cầu rút {payout.Amount:N0} đ",
            $"{businessName ?? "Đối tác"} yêu cầu rút về {payout.BankName} {payout.AccountNumber}.", NotificationType.Payout, "/admin/finance");
        return Ok(payout);
    }

    [HttpGet("payout/my-payouts")]
    [Authorize(Roles = AppRoles.Partner)]
    public async Task<IActionResult> GetMyPayouts()
    {
        var partnerId = await GetCurrentPartnerIdAsync();
        if (partnerId is null) return BadRequest(new { message = "Đối tác không tồn tại." });

        var payouts = await _context.Payouts
            .Where(p => p.PartnerId == partnerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(payouts);
    }

    [HttpGet("payout/all")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAllPayouts()
    {
        var payouts = await _context.Payouts
            .Include(p => p.Partner)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PayoutResponseDto
            {
                Id = p.Id,
                PartnerId = p.PartnerId,
                BusinessName = p.Partner != null ? p.Partner.BusinessName : "Unknown",
                Amount = p.Amount,
                BankName = p.BankName,
                AccountNumber = p.AccountNumber,
                AccountHolder = p.AccountHolder,
                Status = p.Status,
                ProofImageUrl = p.ProofImageUrl,
                AdminNote = p.AdminNote,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                PaidAt = p.PaidAt
            })
            .ToListAsync();

        return Ok(payouts);
    }

    /// <summary>Admin duyệt (paid, kèm chứng từ) hoặc từ chối (rejected) một yêu cầu rút tiền đang chờ.</summary>
    [HttpPost("payout/{id}/process")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> ProcessPayout(int id, ProcessPayoutDto request)
    {
        var payout = await _context.Payouts.FindAsync(id);
        if (payout == null) return NotFound();

        var decision = request.Status?.Trim().ToLowerInvariant();
        if (decision is null || !PayoutDecisions.Contains(decision))
            return BadRequest(new { message = "Trạng thái phải là 'paid' hoặc 'rejected'." });

        if (!string.Equals(payout.Status, "pending", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Yêu cầu này đã được xử lý." });

        if (decision == "paid" && string.IsNullOrWhiteSpace(request.ProofImageUrl) && string.IsNullOrWhiteSpace(request.TransactionId))
            return BadRequest(new { message = "Cần ảnh chứng từ hoặc mã giao dịch khi xác nhận đã chuyển tiền." });

        var previousStatus = payout.Status;
        payout.Status = decision;
        payout.AdminNote = request.AdminNote;
        payout.ProofImageUrl = request.ProofImageUrl;
        payout.TransactionId = request.TransactionId;
        payout.UpdatedAt = DateTime.Now;
        if (decision == "paid") payout.PaidAt = DateTime.Now;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Payout {PayoutId} marked {Decision} by admin {UserId}", id, decision, User.GetUserId());
        await _audit.LogAsync("PAYOUT_PROCESS", "thanh_toan_doi_tac", id, new { status = previousStatus },
            new { status = decision, payout.Amount, payout.TransactionId, payout.ProofImageUrl, payout.AdminNote });
        var partnerUserId = await _context.Partners.Where(p => p.Id == payout.PartnerId).Select(p => (int?)p.UserId).FirstOrDefaultAsync();
        if (partnerUserId is int uid)
            await _inApp.NotifyAsync(uid,
                decision == "paid" ? $"Đã chuyển {payout.Amount:N0} đ cho bạn" : $"Yêu cầu rút {payout.Amount:N0} đ bị từ chối",
                decision == "paid"
                    ? $"Về {payout.BankName} {payout.AccountNumber}{(string.IsNullOrWhiteSpace(payout.TransactionId) ? "" : $", mã GD {payout.TransactionId}")}."
                    : (string.IsNullOrWhiteSpace(payout.AdminNote) ? "Số tiền đã trả lại số dư khả dụng." : $"Lý do: {payout.AdminNote}"),
                NotificationType.Payout, "/business/payments");
        return Ok(payout);
    }

    /// <summary>Doanh thu đã hoàn tất, tiền đang chờ rút, và số dư còn rút được.</summary>
    private async Task<(decimal completed, decimal pendingPayouts, decimal available)> GetBalanceAsync(int partnerId)
    {
        var completed = await _context.Bookings
            .Where(b => b.Room.Property.PartnerId == partnerId && b.Status == BookingStatus.Completed)
            .SumAsync(b => (decimal?)(b.TotalPrice - (b.ServiceFee ?? 0))) ?? 0;

        var payouts = await _context.Payouts
            .Where(p => p.PartnerId == partnerId && (p.Status == "pending" || p.Status == "paid"))
            .Select(p => new { p.Status, p.Amount })
            .ToListAsync();

        var pending = payouts.Where(p => string.Equals(p.Status, "pending", StringComparison.OrdinalIgnoreCase)).Sum(p => p.Amount);
        var paid = payouts.Where(p => string.Equals(p.Status, "paid", StringComparison.OrdinalIgnoreCase)).Sum(p => p.Amount);

        return (completed, pending, completed - pending - paid);
    }

    private async Task<int?> GetCurrentPartnerIdAsync()
    {
        var userId = User.GetUserId();
        if (userId is null) return null;

        var id = await _context.Partners
            .Where(p => p.UserId == userId.Value)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();
        return id == 0 ? null : id;
    }
}
