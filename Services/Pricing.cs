using HotelManagement.API.Models;

namespace HotelManagement.API.Services;

/// <summary>Báo giá một đơn: tiền phòng (có giá cuối tuần) + phí dịch vụ + thuế.</summary>
public record PriceQuote(
    int Nights,
    int WeekendNights,
    decimal BasePrice,
    decimal? WeekendPrice,
    decimal Subtotal,
    decimal ServiceFeeRate,
    decimal ServiceFee,
    decimal TaxRate,
    decimal Tax,
    decimal Total,
    int PaymentHoldMinutes);

public static class Pricing
{
    /// <summary>Đêm thứ Sáu và thứ Bảy tính giá cuối tuần (nếu chủ nhà đặt).</summary>
    public static bool IsWeekendNight(DateTime night) => night.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday;

    public static PriceQuote Quote(Room room, DateTime checkIn, DateTime checkOut, SystemSettingsSnapshot settings)
    {
        var nights = Math.Max(0, (int)(checkOut.Date - checkIn.Date).TotalDays);
        var weekendPrice = room.WeekendPrice is > 0 ? room.WeekendPrice : null;
        decimal subtotal = 0;
        var weekendNights = 0;
        for (var d = checkIn.Date; d < checkOut.Date; d = d.AddDays(1))
        {
            if (weekendPrice is not null && IsWeekendNight(d))
            {
                subtotal += weekendPrice.Value;
                weekendNights++;
            }
            else
            {
                subtotal += room.BasePrice;
            }
        }

        var serviceFee = Math.Round(subtotal * settings.ServiceFeeRate, 0);
        var tax = Math.Round(subtotal * settings.TaxRate, 0);
        return new PriceQuote(nights, weekendNights, room.BasePrice, weekendPrice, subtotal,
            settings.ServiceFeeRate, serviceFee, settings.TaxRate, tax, subtotal + serviceFee + tax,
            settings.PaymentHoldMinutes);
    }

    /// <summary>Tiền chủ nhà nhận khi khách trả phòng: khách trả trừ phí dịch vụ của StayMaster.</summary>
    public static decimal PartnerEarning(Booking b) => b.TotalPrice - (b.ServiceFee ?? 0);
}
