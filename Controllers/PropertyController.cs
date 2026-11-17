using System.Security.Claims;
using HotelManagement.API.Common;
using HotelManagement.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagement.API.Models;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/properties")]
[Authorize(Roles = AppRoles.Partner)]
public class PropertyController : ControllerBase
{
    private readonly AppDbContext _db;

    public PropertyController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyProperty()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized(new { message = "Bạn cần đăng nhập để thực hiện thao tác này" });
        }

        var partnerId = await _db.Partners
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        if (partnerId == 0)
        {
            return NotFound(new { message = "Bạn chưa đăng ký thông tin đối tác" });
        }

        var property = await _db.Properties
            .FirstOrDefaultAsync(p => p.PartnerId == partnerId);

        if (property == null)
        {
            return NotFound(new { message = "Không tìm thấy cơ sở lưu trú của bạn" });
        }
        return Ok(property);
    }

    [HttpGet("partner")]
    public async Task<IActionResult> GetPartnerProperties()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized(new { message = "Bạn cần đăng nhập để thực hiện thao tác này" });
        }

        var partner = await _db.Partners.FirstOrDefaultAsync(p => p.UserId == userId);
        if (partner == null)
        {
            return NotFound(new { message = "Bạn chưa đăng ký thông tin đối tác" });
        }

        var properties = await _db.Properties
            .Where(p => p.PartnerId == partner.Id)
            .Include(p => p.Rooms)
            .ToListAsync();

        return Ok(properties);
    }

    [HttpPut("my")]
    public async Task<IActionResult> UpdateMyProperty([FromBody] UpdatePropertyRequest req)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized(new { message = "Bạn cần đăng nhập để thực hiện thao tác này" });
        }

        var partner = await _db.Partners.FirstOrDefaultAsync(p => p.UserId == userId);
        if (partner == null)
        {
            return NotFound(new { message = "Bạn chưa đăng ký thông tin đối tác" });
        }

        var property = await _db.Properties.FirstOrDefaultAsync(p => p.PartnerId == partner.Id);
        if (property == null)
        {
            return NotFound(new { message = "Không tìm thấy cơ sở lưu trú để cập nhật" });
        }

        if (string.IsNullOrWhiteSpace(req.Name)) return BadRequest(new { message = "Tên cơ sở không được để trống" });
        if (req.Latitude is not null && (req.Latitude < -90 || req.Latitude > 90))
            return BadRequest(new { message = "Vĩ độ không hợp lệ" });
        if (req.Longitude is not null && (req.Longitude < -180 || req.Longitude > 180))
            return BadRequest(new { message = "Kinh độ không hợp lệ" });
        TimeSpan? checkIn = null, checkOut = null;
        if (!string.IsNullOrWhiteSpace(req.CheckInTime))
        {
            if (!TimeSpan.TryParse(req.CheckInTime, out var t)) return BadRequest(new { message = "Giờ nhận phòng không hợp lệ" });
            checkIn = t;
        }
        if (!string.IsNullOrWhiteSpace(req.CheckOutTime))
        {
            if (!TimeSpan.TryParse(req.CheckOutTime, out var t)) return BadRequest(new { message = "Giờ trả phòng không hợp lệ" });
            checkOut = t;
        }

        property.Name = req.Name.Trim();
        property.Type = req.Type?.Trim();
        property.City = req.City?.Trim();
        property.District = req.District?.Trim();
        property.Ward = req.Ward?.Trim();
        property.DetailedAddress = req.DetailedAddress?.Trim();
        property.Description = req.Description?.Trim();
        if (req.ImageUrl is not null) property.ImageUrl = req.ImageUrl;
        if (req.Latitude is not null && req.Longitude is not null)
        {
            property.Latitude = req.Latitude;
            property.Longitude = req.Longitude;
        }
        if (checkIn is not null) property.DefaultCheckInTime = checkIn;
        if (checkOut is not null) property.DefaultCheckOutTime = checkOut;
        property.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Cập nhật thông tin cơ sở thành công", property });
    }
}

public class UpdatePropertyRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Ward { get; set; }
    public string? DetailedAddress { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    /// <summary>"14:00"</summary>
    public string? CheckInTime { get; set; }
    /// <summary>"12:00"</summary>
    public string? CheckOutTime { get; set; }
}
