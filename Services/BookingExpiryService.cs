namespace HotelManagement.API.Services;

/// <summary>
/// Mỗi phút huỷ các đơn "chờ thanh toán" đã quá thời gian giữ phòng (mặc định 30 phút, sửa trong Cấu hình),
/// để phòng không bị giữ vô thời hạn.
/// </summary>
public class BookingExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<BookingExpiryService> _logger;

    public BookingExpiryService(IServiceScopeFactory scopes, ILogger<BookingExpiryService> logger)
    {
        _scopes = scopes;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Chờ app khởi động xong
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        do
        {
            try
            {
                using var scope = _scopes.CreateScope();
                var bookings = scope.ServiceProvider.GetRequiredService<IBookingService>();
                await bookings.ExpireUnpaidAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Booking expiry run failed");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
