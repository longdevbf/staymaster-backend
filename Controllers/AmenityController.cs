using HotelManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/amenities")]
public class AmenityController : ControllerBase
{
    private readonly IRoomService _roomService;
    public AmenityController(IRoomService roomService) => _roomService = roomService;

    // GET /api/amenities — danh sách tiện ích để Đối tác chọn khi đăng phòng
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _roomService.GetAmenitiesAsync());
}
