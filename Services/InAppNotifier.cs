using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Services;

public static class NotificationType
{
    public const string Booking = "booking";
    public const string Inbox = "inbox";
    public const string Payout = "payout";
    public const string Room = "room";
    public const string Review = "review";
    public const string System = "system";
}

/// <summary>
/// Thông báo trong app (chuông trên đầu trang). Ghi bằng DbContext riêng và không bao giờ ném lỗi,
/// để thao tác chính không hỏng vì thông báo.
/// </summary>
public interface IInAppNotifier
{
    Task NotifyAsync(int userId, string title, string? content, string type, string? link = null);
    Task NotifyManyAsync(IEnumerable<int> userIds, string title, string? content, string type, string? link = null);
    /// <summary>Gửi cho mọi tài khoản quản trị</summary>
    Task NotifyAdminsAsync(string title, string? content, string type, string? link = null);
    /// <summary>Gửi cho chủ nhà sở hữu phòng</summary>
    Task NotifyRoomOwnerAsync(int roomId, string title, string? content, string type, string? link = null);
}

public class InAppNotifier : IInAppNotifier
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<InAppNotifier> _logger;

    public InAppNotifier(IServiceScopeFactory scopes, ILogger<InAppNotifier> logger)
    {
        _scopes = scopes;
        _logger = logger;
    }

    public Task NotifyAsync(int userId, string title, string? content, string type, string? link = null) =>
        NotifyManyAsync([userId], title, content, type, link);

    public async Task NotifyManyAsync(IEnumerable<int> userIds, string title, string? content, string type, string? link = null)
    {
        try
        {
            var ids = userIds.Distinct().ToList();
            if (ids.Count == 0) return;
            using var scope = _scopes.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            foreach (var id in ids)
            {
                db.Notifications.Add(new Notification
                {
                    UserId = id,
                    Title = Truncate(title, 200),
                    Content = content is null ? null : Truncate(content, 1000),
                    Type = type,
                    Link = link is null ? null : Truncate(link, 255),
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                });
            }
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not save in-app notification {Title}", title);
        }
    }

    public async Task NotifyAdminsAsync(string title, string? content, string type, string? link = null)
    {
        try
        {
            using var scope = _scopes.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roles = await db.UserRoles
                .Where(ur => ur.User.Status != UserStatus.Locked)
                .Select(ur => new { ur.UserId, ur.Role.RoleName })
                .ToListAsync();
            var adminIds = roles.Where(r => AppRoles.FromDbRoleName(r.RoleName) == AppRoles.Admin).Select(r => r.UserId);
            await NotifyManyAsync(adminIds, title, content, type, link);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not notify admins {Title}", title);
        }
    }

    public async Task NotifyRoomOwnerAsync(int roomId, string title, string? content, string type, string? link = null)
    {
        try
        {
            using var scope = _scopes.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var ownerId = await db.Rooms.Where(r => r.Id == roomId)
                .Select(r => (int?)r.Property.Partner.UserId)
                .FirstOrDefaultAsync();
            if (ownerId is not null) await NotifyAsync(ownerId.Value, title, content, type, link);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not notify owner of room {RoomId}", roomId);
        }
    }

    private static string Truncate(string value, int max) => value.Length <= max ? value : value[..(max - 1)] + "…";
}
