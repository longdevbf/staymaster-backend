using System.Text.Json;
using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.Models;

namespace HotelManagement.API.Services;

/// <summary>
/// Ghi nhật ký thao tác vào log_he_thong (SRS 3f): ai làm gì, trên bản ghi nào, trước/sau ra sao.
/// Không bao giờ làm hỏng thao tác chính: lỗi ghi nhật ký chỉ được ghi ra log ứng dụng.
/// </summary>
public interface IAuditService
{
    Task LogAsync(string action, string? table = null, object? key = null, object? oldData = null, object? newData = null, int? userId = null);
}

public class AuditService : IAuditService
{
    private static readonly JsonSerializerOptions Json = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly IServiceScopeFactory _scopes;
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<AuditService> _logger;

    public AuditService(IServiceScopeFactory scopes, IHttpContextAccessor http, ILogger<AuditService> logger)
    {
        _scopes = scopes;
        _http = http;
        _logger = logger;
    }

    public async Task LogAsync(string action, string? table = null, object? key = null, object? oldData = null, object? newData = null, int? userId = null)
    {
        try
        {
            // DbContext riêng để không lẫn vào (hoặc làm hỏng) transaction của thao tác chính
            using var scope = _scopes.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.AuditLogs.Add(new AuditLog
            {
                UserId = userId ?? _http.HttpContext?.User.GetUserId(),
                Action = Truncate(action, 100)!,
                TableName = Truncate(table, 100),
                EntityKey = Truncate(key?.ToString(), 100),
                OldData = Serialize(oldData),
                NewData = Serialize(newData),
                CreatedAt = DateTime.Now,
            });
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not write audit log {Action} {Table} {Key}", action, table, key);
        }
    }

    private static string? Serialize(object? value) => value switch
    {
        null => null,
        string s => s,
        _ => JsonSerializer.Serialize(value, Json),
    };

    private static string? Truncate(string? value, int max) =>
        value is null || value.Length <= max ? value : value[..max];
}
