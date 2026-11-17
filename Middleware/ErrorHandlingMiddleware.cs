using System.Net;
using System.Text.Json;

namespace HotelManagement.API.Middleware;

/// <summary>
/// Bắt lỗi chưa xử lý, ghi log và trả JSON. Chi tiết lỗi (stack trace) chỉ trả về khi chạy Development.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
            if (context.Response.HasStarted) throw;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            object response = _env.IsDevelopment()
                ? new { message = "Có lỗi xảy ra, vui lòng thử lại", detail = ex.ToString() }
                : new { message = "Có lỗi xảy ra, vui lòng thử lại" };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
