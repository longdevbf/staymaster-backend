using System.Globalization;
using HotelManagement.API.Data;
using HotelManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Services;

/// <summary>Giá trị cấu hình đang dùng. Admin sửa trong trang Cấu hình; chưa sửa thì lấy mặc định.</summary>
public record SystemSettingsSnapshot(
    decimal ServiceFeeRate,
    decimal TaxRate,
    int PaymentHoldMinutes,
    string SupportEmail,
    string SupportPhone);

public interface ISystemSettings
{
    Task<SystemSettingsSnapshot> GetAsync();
    /// <summary>Trả về lỗi tiếng Việt nếu giá trị không hợp lệ</summary>
    Task<string?> UpdateAsync(SystemSettingsSnapshot values, int? userId);
}

public class SystemSettingsService : ISystemSettings
{
    public const string ServiceFeeRateKey = "service_fee_rate";
    public const string TaxRateKey = "tax_rate";
    public const string PaymentHoldMinutesKey = "payment_hold_minutes";
    public const string SupportEmailKey = "support_email";
    public const string SupportPhoneKey = "support_phone";

    private static readonly TimeSpan CacheFor = TimeSpan.FromSeconds(30);

    private readonly IServiceScopeFactory _scopes;
    private readonly IConfiguration _config;
    private readonly ILogger<SystemSettingsService> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private SystemSettingsSnapshot? _cached;
    private DateTime _cachedAt;

    public SystemSettingsService(IServiceScopeFactory scopes, IConfiguration config, ILogger<SystemSettingsService> logger)
    {
        _scopes = scopes;
        _config = config;
        _logger = logger;
    }

    private SystemSettingsSnapshot Defaults() => new(
        ServiceFeeRate: 0.10m,
        TaxRate: 0.05m,
        PaymentHoldMinutes: _config.GetValue("Booking:PaymentHoldMinutes", 30),
        SupportEmail: _config["App:SupportEmail"] ?? "support@staymaster.vn",
        SupportPhone: _config["App:SupportPhone"] ?? "1900 0000");

    public async Task<SystemSettingsSnapshot> GetAsync()
    {
        if (_cached is not null && DateTime.UtcNow - _cachedAt < CacheFor) return _cached;
        await _lock.WaitAsync();
        try
        {
            if (_cached is not null && DateTime.UtcNow - _cachedAt < CacheFor) return _cached;
            var d = Defaults();
            Dictionary<string, string> rows;
            try
            {
                using var scope = _scopes.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                rows = await db.SystemSettings.AsNoTracking().ToDictionaryAsync(x => x.Key, x => x.Value);
            }
            catch (Exception ex)
            {
                // Bảng chưa có (chưa chạy migration) thì vẫn chạy bằng giá trị mặc định
                _logger.LogWarning(ex, "Could not read system settings; using defaults");
                rows = new();
            }

            _cached = new SystemSettingsSnapshot(
                ServiceFeeRate: Dec(rows, ServiceFeeRateKey) ?? d.ServiceFeeRate,
                TaxRate: Dec(rows, TaxRateKey) ?? d.TaxRate,
                PaymentHoldMinutes: (int?)Dec(rows, PaymentHoldMinutesKey) ?? d.PaymentHoldMinutes,
                SupportEmail: rows.GetValueOrDefault(SupportEmailKey) is { Length: > 0 } e ? e : d.SupportEmail,
                SupportPhone: rows.GetValueOrDefault(SupportPhoneKey) is { Length: > 0 } p ? p : d.SupportPhone);
            _cachedAt = DateTime.UtcNow;
            return _cached;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<string?> UpdateAsync(SystemSettingsSnapshot v, int? userId)
    {
        if (v.ServiceFeeRate is < 0 or > 0.5m) return "Phí dịch vụ phải từ 0% đến 50%";
        if (v.TaxRate is < 0 or > 0.3m) return "Thuế phải từ 0% đến 30%";
        if (v.PaymentHoldMinutes is < 5 or > 1440) return "Thời gian giữ phòng phải từ 5 đến 1440 phút";
        if (string.IsNullOrWhiteSpace(v.SupportEmail) || !v.SupportEmail.Contains('@')) return "Email hỗ trợ không hợp lệ";
        if (string.IsNullOrWhiteSpace(v.SupportPhone)) return "Số điện thoại hỗ trợ không được để trống";

        using var scope = _scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var values = new Dictionary<string, string>
        {
            [ServiceFeeRateKey] = v.ServiceFeeRate.ToString(CultureInfo.InvariantCulture),
            [TaxRateKey] = v.TaxRate.ToString(CultureInfo.InvariantCulture),
            [PaymentHoldMinutesKey] = v.PaymentHoldMinutes.ToString(CultureInfo.InvariantCulture),
            [SupportEmailKey] = v.SupportEmail.Trim(),
            [SupportPhoneKey] = v.SupportPhone.Trim(),
        };
        var existing = await db.SystemSettings.ToDictionaryAsync(x => x.Key);
        foreach (var (key, value) in values)
        {
            if (existing.TryGetValue(key, out var row))
            {
                row.Value = value;
                row.UpdatedAt = DateTime.Now;
                row.UpdatedBy = userId;
            }
            else
            {
                db.SystemSettings.Add(new SystemSetting { Key = key, Value = value, UpdatedAt = DateTime.Now, UpdatedBy = userId });
            }
        }
        await db.SaveChangesAsync();
        _cached = null;
        return null;
    }

    private static decimal? Dec(Dictionary<string, string> rows, string key) =>
        rows.TryGetValue(key, out var s) && decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var d) ? d : null;
}
