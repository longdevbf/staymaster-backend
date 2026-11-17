using HotelManagement.API.Common;
using HotelManagement.API.DTOs.Booking;
using HotelManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IInvoiceService _invoiceService;

    public BookingController(IBookingService bookingService, IInvoiceService invoiceService)
    {
        _bookingService = bookingService;
        _invoiceService = invoiceService;
    }

    // POST /api/bookings
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest req)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var (booking, error) = await _bookingService.CreateBookingAsync(userId.Value, req);
        if (error is not null) return BadRequest(new { message = error });

        return Ok(booking);
    }

    // GET /api/rooms/{roomId}/quote?checkIn=&checkOut=  — báo giá (công khai)
    [HttpGet("~/api/rooms/{roomId:int}/quote")]
    [AllowAnonymous]
    public async Task<IActionResult> Quote(int roomId, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
    {
        var (quote, error) = await _bookingService.QuoteAsync(roomId, checkIn, checkOut);
        return error is not null ? BadRequest(new { message = error }) : Ok(quote);
    }

    // GET /api/rooms/{roomId}/booked-ranges — các khoảng ngày đã kín (công khai, không lộ thông tin khách)
    [HttpGet("~/api/rooms/{roomId:int}/booked-ranges")]
    [AllowAnonymous]
    public async Task<IActionResult> BookedRanges(int roomId) =>
        Ok(await _bookingService.GetBookedRangesAsync(roomId));

    // GET /api/bookings/my
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var bookings = await _bookingService.GetUserBookingsAsync(userId.Value);
        return Ok(bookings);
    }

    // GET /api/bookings  (Admin)
    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _bookingService.GetAllBookingsAsync();
        return Ok(bookings);
    }

    // GET /api/bookings/{id}  (khách đặt, đối tác sở hữu phòng, admin)
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        var (_, denied) = await AuthorizeBookingAsync(id, customer: true, partner: true);
        if (denied is not null) return denied;

        var booking = await _bookingService.GetBookingByIdAsync(id);
        return booking is null ? NotFound() : Ok(booking);
    }

    // GET /api/bookings/{id}/invoice  — hoá đơn PDF khi đơn đã hoàn tất (khách đặt, đối tác, admin)
    [HttpGet("{id:int}/invoice")]
    public async Task<IActionResult> GetInvoice(int id)
    {
        var (_, denied) = await AuthorizeBookingAsync(id, customer: true, partner: true);
        if (denied is not null) return denied;

        var invoice = await _invoiceService.GenerateAsync(id);
        if (invoice is null)
            return BadRequest(new { message = "Hoá đơn chỉ có khi chuyến đi đã hoàn tất" });

        return File(invoice.Value.pdf, "application/pdf", invoice.Value.fileName);
    }

    // GET /api/bookings/{id}/sync  (khách đặt, admin) — kiểm tra thanh toán SePay
    [HttpGet("{id:int}/sync")]
    public async Task<IActionResult> SyncBookingPayment(int id)
    {
        var (_, denied) = await AuthorizeBookingAsync(id, customer: true, partner: false);
        if (denied is not null) return denied;

        var booking = await _bookingService.SyncSePayPaymentAsync(id);
        return booking is null ? NotFound() : Ok(booking);
    }

    // PUT /api/bookings/{id}/confirm  (đối tác sở hữu phòng, admin)
    [HttpPut("{id:int}/confirm")]
    public async Task<IActionResult> ConfirmBooking(int id)
    {
        var (_, denied) = await AuthorizeBookingAsync(id, customer: false, partner: true);
        if (denied is not null) return denied;

        var result = await _bookingService.ConfirmBookingAsync(id);
        if (result is null)
            return BadRequest(new { message = "Chỉ xác nhận được đơn đang chờ duyệt" });

        return Ok(result);
    }

    // PUT /api/bookings/{id}/cancel
    // Khách huỷ được đơn của mình khi đang chờ duyệt; đối tác sở hữu phòng và admin huỷ được đơn chưa nhận phòng.
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> CancelBooking(int id, [FromBody] CancelBookingRequest? req = null)
    {
        var (info, denied) = await AuthorizeBookingAsync(id, customer: true, partner: true);
        if (denied is not null) return denied;

        var userId = User.GetUserId();
        var actsAsCustomerOnly = !User.IsAdmin() && info!.PartnerUserId != userId;
        if (actsAsCustomerOnly && !BookingStatus.Is(info!.Status, BookingStatus.Pending))
            return BadRequest(new { message = "Chỉ huỷ được đơn đang chờ duyệt" });

        var result = await _bookingService.CancelBookingAsync(id, req?.Reason?.Trim());
        if (result is null)
            return BadRequest(new { message = "Không thể huỷ đơn đã nhận phòng hoặc đã kết thúc" });

        return Ok(result);
    }

    // GET /api/bookings/occupancy  (Admin)
    [HttpGet("occupancy")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetOccupancyRates(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        if (from >= to)
            return BadRequest(new { message = "Ngày bắt đầu phải trước ngày kết thúc" });

        var result = await _bookingService.GetOccupancyRatesAsync(from, to);
        return Ok(result);
    }

    // GET /api/bookings/partner  (Đối tác)
    [HttpGet("partner")]
    [Authorize(Roles = AppRoles.Partner)]
    public async Task<IActionResult> GetPartnerBookings()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var bookings = await _bookingService.GetPartnerBookingsByUserIdAsync(userId.Value);
        return Ok(bookings);
    }

    // PUT /api/bookings/{id}/checkin  (đối tác sở hữu phòng, admin)
    [HttpPut("{id:int}/checkin")]
    public async Task<IActionResult> CheckIn(int id)
    {
        var (_, denied) = await AuthorizeBookingAsync(id, customer: false, partner: true);
        if (denied is not null) return denied;

        var (result, error) = await _bookingService.CheckInAsync(id);
        if (error is not null) return BadRequest(new { message = error });
        return Ok(result);
    }

    // PUT /api/bookings/{id}/complete  (đối tác sở hữu phòng, admin)
    [HttpPut("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        var (_, denied) = await AuthorizeBookingAsync(id, customer: false, partner: true);
        if (denied is not null) return denied;

        var (result, error) = await _bookingService.CompleteAsync(id);
        if (error is not null) return BadRequest(new { message = error });
        return Ok(result);
    }

    /// <summary>
    /// Admin luôn được phép. Ngoài ra: khách đặt đơn (nếu <paramref name="customer"/>)
    /// và đối tác sở hữu phòng (nếu <paramref name="partner"/>).
    /// </summary>
    private async Task<(BookingAccessInfo? info, IActionResult? denied)> AuthorizeBookingAsync(
        int bookingId, bool customer, bool partner)
    {
        var info = await _bookingService.GetAccessInfoAsync(bookingId);
        if (info is null) return (null, NotFound(new { message = "Không tìm thấy đơn đặt phòng" }));

        if (User.IsAdmin()) return (info, null);

        var userId = User.GetUserId();
        if (userId is null) return (info, Unauthorized());
        if (customer && info.CustomerUserId == userId) return (info, null);
        if (partner && info.PartnerUserId == userId) return (info, null);

        return (info, StatusCode(StatusCodes.Status403Forbidden,
            new { message = "Bạn không có quyền thao tác trên đơn này" }));
    }
}
