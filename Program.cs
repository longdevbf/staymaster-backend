using System.Text;
using System.Text.Json.Serialization;
using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.Middleware;
using HotelManagement.API.Models;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// QuestPDF Community: miễn phí cho dự án/doanh nghiệp doanh thu dưới 1 triệu USD/năm
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// ── Cấu hình bắt buộc ────────────────────────────────────────
// Không để khoá bí mật trong appsettings.json. Đặt qua biến môi trường
// (ConnectionStrings__DefaultConnection, Jwt__SecretKey, ...) hoặc appsettings.Development.json / dotnet user-secrets.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection. See README for configuration.");

var jwtKey = builder.Configuration["Jwt:SecretKey"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Jwt:SecretKey must be set and at least 32 bytes long. See README for configuration.");

// Database - kết nối QL_NhaNghi có sẵn
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// CORS - danh sách origin của frontend lấy từ App:CorsOrigins
var corsOrigins = builder.Configuration.GetSection("App:CorsOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .WithExposedHeaders("Content-Disposition")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        // Tài khoản bị khoá: từ chối cả token cũ (kiểm tra DB, nhớ kết quả 30 giây)
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var userId = ctx.Principal?.GetUserId();
                if (userId is null) return;
                var cache = ctx.HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
                var locked = await cache.GetOrCreateAsync($"user-locked:{userId}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
                    var db = ctx.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                    var status = await db.Users.Where(u => u.Id == userId).Select(u => u.Status).FirstOrDefaultAsync();
                    return UserStatus.IsLocked(status);
                });
                if (locked) ctx.Fail("Tài khoản đã bị khoá");
            },
        };
    });
builder.Services.AddMemoryCache();

builder.Services.AddAuthorization();

// Dependency Injection
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IInboxService, InboxService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuditService, AuditService>();
builder.Services.AddSingleton<IInAppNotifier, InAppNotifier>();
builder.Services.AddSingleton<ISystemSettings, SystemSettingsService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
if (!args.Contains("--seed-demo"))
    builder.Services.AddHostedService<BookingExpiryService>();

// AI chatbot (SRS 2e): nhà cung cấp chọn qua Ai:Provider — "anthropic", "gemini", "deepseek", "offline" hoặc rỗng (tắt)
builder.Services.AddMemoryCache();
builder.Services.Configure<HotelManagement.API.Services.Ai.AiOptions>(builder.Configuration.GetSection("Ai"));
builder.Services.AddSingleton<HotelManagement.API.Services.Ai.IChatModel>(sp =>
{
    var ai = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<HotelManagement.API.Services.Ai.AiOptions>>().Value;
    var log = sp.GetRequiredService<ILogger<Program>>();
    switch (ai.Provider?.Trim().ToLowerInvariant())
    {
        case "anthropic" when !string.IsNullOrWhiteSpace(ai.ApiKey):
            return new HotelManagement.API.Services.Ai.ClaudeChatModel(ai);
        case "gemini" when !string.IsNullOrWhiteSpace(ai.ApiKey):
            return new HotelManagement.API.Services.Ai.GeminiChatModel(ai, sp.GetRequiredService<IHttpClientFactory>().CreateClient("gemini"));
        case "deepseek" when !string.IsNullOrWhiteSpace(ai.ApiKey):
            return new HotelManagement.API.Services.Ai.DeepSeekChatModel(ai, sp.GetRequiredService<IHttpClientFactory>().CreateClient("deepseek"));
        case "anthropic" or "gemini" or "deepseek":
            log.LogWarning("Ai:Provider is {Provider} but Ai:ApiKey is empty — chatbot disabled", ai.Provider);
            return new HotelManagement.API.Services.Ai.DisabledChatModel();
        case "offline":
            return new HotelManagement.API.Services.Ai.OfflineChatModel();
        default:
            log.LogWarning("Ai:Provider not configured — chatbot answers with the fallback message");
            return new HotelManagement.API.Services.Ai.DisabledChatModel();
    }
});
builder.Services.AddScoped<HotelManagement.API.Services.Ai.IChatService, HotelManagement.API.Services.Ai.ChatService>();

// Giới hạn chatbot: 20 tin nhắn / phút / IP, tránh lạm dụng API key
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("chat", context => System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(1) }));
});

// Email: hàng đợi + gửi nền (SRS 2c, 3a)
builder.Services.Configure<HotelManagement.API.Services.Email.EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.AddSingleton<HotelManagement.API.Services.Email.EmailQueue>();
builder.Services.AddSingleton<HotelManagement.API.Services.Email.IEmailQueue>(sp => sp.GetRequiredService<HotelManagement.API.Services.Email.EmailQueue>());
builder.Services.AddHostedService<HotelManagement.API.Services.Email.EmailBackgroundSender>();
builder.Services.AddScoped<HotelManagement.API.Services.Email.INotificationService, HotelManagement.API.Services.Email.NotificationService>();

builder.Services.AddHttpClient();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Nhập 'Bearer ' và dán Token của bạn vào.\n\nVí dụ: \"Bearer eyJhbGciOiJIUzI...\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Chạy sau reverse proxy (Caddy/nginx trong docker-compose): lấy IP thật của khách từ X-Forwarded-For,
// nếu không giới hạn 20 tin nhắn/phút của chatbot sẽ tính chung cho mọi người. Chỉ bật khi API không mở thẳng ra
// Internet (docker-compose chỉ mở cổng API cho 127.0.0.1), vì header này giả được.
if (builder.Configuration.GetValue<bool>("App:TrustForwardedHeaders"))
{
    var forwarded = new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto };
    forwarded.KnownNetworks.Clear();
    forwarded.KnownProxies.Clear();
    app.UseForwardedHeaders(forwarded);
}

// `dotnet run -- --seed-demo`: xoá sạch dữ liệu và tạo dữ liệu mẫu (Data/DemoSeeder.cs). Chỉ chạy ở Development.
if (args.Contains("--seed-demo"))
{
    if (!app.Environment.IsDevelopment())
    {
        Console.Error.WriteLine("Từ chối seed: lệnh này xoá toàn bộ dữ liệu, chỉ chạy với ASPNETCORE_ENVIRONMENT=Development.");
        return;
    }
    using var seedScope = app.Services.CreateScope();
    var seedDb = seedScope.ServiceProvider.GetRequiredService<AppDbContext>();
    await seedDb.Database.MigrateAsync();
    // Có SeedAdmin:Password thì admin mẫu dùng mật khẩu đó (server public không nên để Admin@123)
    await DemoSeeder.RunAsync(seedDb, seedScope.ServiceProvider.GetRequiredService<ILogger<Program>>(),
        builder.Configuration["SeedAdmin:Password"]);
    return;
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles(); // Ảnh phòng trong wwwroot/uploads

app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Seeding data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    // Docker: DB tạo từ database/QL_NhaNghi.sql, các migration sau đó áp dụng khi khởi động.
    // Lỗi migration thì dừng hẳn (để container khởi động lại), không chạy tiếp với DB thiếu cột.
    if (builder.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
    {
        logger.LogInformation("Applying EF migrations (Database:MigrateOnStartup)");
        await services.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    }
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // Đảm bảo các vai trò cơ bản tồn tại
        var roles = new[] { "admin", "customer", "partner" };
        foreach (var roleName in roles)
        {
            if (!await context.Roles.AnyAsync(r => r.RoleName == roleName))
            {
                context.Roles.Add(new Role { RoleName = roleName });
            }
        }
        await context.SaveChangesAsync();

        // Tài khoản admin đầu tiên: chỉ tạo khi đã cấu hình SeedAdmin:Email và SeedAdmin:Password
        var adminEmail = builder.Configuration["SeedAdmin:Email"];
        var adminPassword = builder.Configuration["SeedAdmin:Password"];
        if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
        {
            var authService = services.GetRequiredService<IAuthService>();
            await authService.EnsureAdminUserAsync(adminEmail, adminPassword);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Đã xảy ra lỗi khi seeding database.");
    }
}

app.MapControllers();

app.Run();

/// <summary>Cho phép project test tham chiếu tới Program.</summary>
public partial class Program { }
