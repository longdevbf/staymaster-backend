using HotelManagement.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/upload")]
[Authorize]
public class UploadController : ControllerBase
{
    private const long MaxBytes = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadController> _logger;

    public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger)
    {
        _env = env;
        _logger = logger;
    }

    // POST /api/upload  — chỉ nhận ảnh, tối đa 5 MB
    [HttpPost]
    [RequestSizeLimit(MaxBytes + 64 * 1024)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Không tìm thấy file tải lên." });

        if (file.Length > MaxBytes)
            return BadRequest(new { message = "Ảnh tối đa 5 MB." });

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Chỉ nhận ảnh JPG, PNG, WEBP hoặc GIF." });

        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var uploadFolder = Path.Combine(webRoot, "uploads", "rooms");
        Directory.CreateDirectory(uploadFolder);

        // Tên file do server tạo, không dùng tên gốc của người dùng
        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var filePath = Path.Combine(uploadFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.CreateNew))
        {
            await file.CopyToAsync(stream);
        }

        _logger.LogInformation("User {UserId} uploaded {FileName} ({Bytes} bytes)", User.GetUserId(), fileName, file.Length);

        var url = $"{Request.Scheme}://{Request.Host}/uploads/rooms/{fileName}";
        return Ok(new { url });
    }
}
