using System.Globalization;
using HotelManagement.API.Common;
using HotelManagement.API.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HotelManagement.API.Services;

public interface IInvoiceService
{
    /// <summary>PDF hoá đơn của đơn đã hoàn tất; null nếu không có đơn hoặc đơn chưa hoàn tất.</summary>
    Task<(byte[] pdf, string fileName)?> GenerateAsync(int bookingId);
}

/// <summary>Hoá đơn PDF chi tiết: tiền phòng, phí dọn dẹp, phí dịch vụ, thuế (SRS 2d, mục tiêu &lt; 5 giây).</summary>
public class InvoiceService : IInvoiceService
{
    private static readonly CultureInfo Vi = new("vi-VN");
    // Be Vietnam Pro (Fonts/, giấy phép OFL) đủ dấu tiếng Việt; QuestPDF tự nạp font trong thư mục ứng dụng
    private static readonly string[] Fonts = { "Be Vietnam Pro", "Lato" };
    private const string Primary = "#135bec";

    private readonly AppDbContext _db;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(AppDbContext db, ILogger<InvoiceService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<(byte[] pdf, string fileName)?> GenerateAsync(int bookingId)
    {
        var b = await _db.Bookings
            .Where(x => x.Id == bookingId)
            .Select(x => new
            {
                x.Id,
                x.Status,
                x.CheckIn,
                x.CheckOut,
                x.Nights,
                x.GuestCount,
                x.PricePerNight,
                x.CleaningFee,
                x.ServiceFee,
                x.Tax,
                x.TotalPrice,
                x.Deposit,
                x.CreatedAt,
                x.UpdatedAt,
                CustomerName = x.User.FullName,
                CustomerEmail = x.User.Email,
                CustomerPhone = x.User.Phone,
                RoomName = x.Room.Name,
                RoomCode = x.Room.RoomCode,
                RoomType = x.Room.RoomType,
                PropertyName = x.Room.Property.Name,
                PropertyAddress = x.Room.Property.DetailedAddress + ", " + x.Room.Property.Ward + ", " +
                                  x.Room.Property.District + ", " + x.Room.Property.City,
                BusinessName = x.Room.Property.Partner != null ? x.Room.Property.Partner.BusinessName : null,
            })
            .FirstOrDefaultAsync();

        if (b is null || !BookingStatus.Is(b.Status, BookingStatus.Completed)) return null;

        var started = DateTime.UtcNow;
        var flat = b.PricePerNight * b.Nights;
        // Đơn mới tách phí: tiền phòng = tổng − phí − thuế (có thể gồm giá cuối tuần)
        var subtotal = b.ServiceFee is not null || b.Tax is not null
            ? b.TotalPrice - (b.CleaningFee ?? 0) - (b.ServiceFee ?? 0) - (b.Tax ?? 0)
            : flat;
        var roomLabel = subtotal == flat
            ? $"Tiền phòng ({Money(b.PricePerNight)} × {b.Nights} đêm)"
            : $"Tiền phòng ({b.Nights} đêm, gồm giá cuối tuần)";
        var lines = new List<(string Label, decimal Amount)>
        {
            (roomLabel, subtotal),
        };
        if (b.CleaningFee is > 0) lines.Add(("Phí dọn dẹp", b.CleaningFee.Value));
        if (b.ServiceFee is > 0) lines.Add(("Phí dịch vụ", b.ServiceFee.Value));
        if (b.Tax is > 0) lines.Add(("Thuế", b.Tax.Value));
        // Đơn cũ không tách phí: phần chênh lệch còn lại hiển thị là "Phụ phí khác"
        var other = b.TotalPrice - lines.Sum(l => l.Amount);
        if (other > 0) lines.Add(("Phụ phí khác", other));

        var invoiceNo = $"INV-{b.Id:D6}";
        var issued = b.UpdatedAt ?? DateTime.Now;

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(t => t.FontFamily(Fonts).FontSize(10).FontColor(Colors.Grey.Darken4));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("StayMaster").FontSize(22).Bold().FontColor(Primary);
                        col.Item().Text("Nền tảng đặt phòng lưu trú").FontColor(Colors.Grey.Darken1);
                    });
                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().AlignRight().Text("HOÁ ĐƠN THANH TOÁN").FontSize(14).Bold();
                        col.Item().AlignRight().Text($"Số: {invoiceNo}");
                        col.Item().AlignRight().Text($"Ngày lập: {issued:dd/MM/yyyy}");
                    });
                });

                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Spacing(16);

                    col.Item().Row(row =>
                    {
                        row.Spacing(16);
                        row.RelativeItem().Element(Box).Column(c =>
                        {
                            c.Item().Text("Khách hàng").Bold().FontColor(Primary);
                            c.Item().Text(b.CustomerName);
                            c.Item().Text(b.CustomerEmail);
                            if (!string.IsNullOrWhiteSpace(b.CustomerPhone)) c.Item().Text(b.CustomerPhone);
                        });
                        row.RelativeItem().Element(Box).Column(c =>
                        {
                            c.Item().Text("Cơ sở lưu trú").Bold().FontColor(Primary);
                            c.Item().Text(b.PropertyName);
                            if (!string.IsNullOrWhiteSpace(b.BusinessName) && b.BusinessName != b.PropertyName)
                                c.Item().Text(b.BusinessName);
                            c.Item().Text(b.PropertyAddress);
                        });
                    });

                    col.Item().Element(Box).Table(t =>
                    {
                        t.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); });
                        void Cell(string label, string value) => t.Cell().Column(c =>
                        {
                            c.Item().Text(label).FontSize(8).FontColor(Colors.Grey.Darken1);
                            c.Item().Text(value).Bold();
                        });
                        Cell("Mã đơn", $"#{b.Id}");
                        Cell("Phòng", $"{b.RoomName} ({b.RoomCode})");
                        Cell("Nhận phòng", b.CheckIn.ToString("dd/MM/yyyy"));
                        Cell("Trả phòng", b.CheckOut.ToString("dd/MM/yyyy"));
                        Cell("Loại phòng", b.RoomType ?? "—");
                        Cell("Số khách", b.GuestCount.ToString());
                        Cell("Số đêm", b.Nights.ToString());
                        Cell("Ngày đặt", (b.CreatedAt ?? b.CheckIn).ToString("dd/MM/yyyy"));
                    });

                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c => { c.RelativeColumn(3); c.RelativeColumn(1); });
                        t.Header(h =>
                        {
                            h.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(6).Text("Khoản mục").Bold();
                            h.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(6).AlignRight().Text("Thành tiền").Bold();
                        });
                        foreach (var (label, amount) in lines)
                        {
                            t.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(6).Text(label);
                            t.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(6).AlignRight().Text(Money(amount));
                        }
                        t.Cell().PaddingTop(10).Text("TỔNG CỘNG").Bold().FontSize(12);
                        t.Cell().PaddingTop(10).AlignRight().Text(Money(b.TotalPrice)).Bold().FontSize(12).FontColor(Primary);
                        t.Cell().Text("Đã thanh toán").FontColor(Colors.Grey.Darken1);
                        t.Cell().AlignRight().Text(Money(b.TotalPrice)).FontColor(Colors.Grey.Darken1);
                    });

                    col.Item().Text("Trạng thái: Đã hoàn thành chuyến đi").FontColor(Colors.Green.Darken2).Bold();
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Cảm ơn bạn đã lựa chọn StayMaster · Trang ").FontSize(8).FontColor(Colors.Grey.Darken1);
                    t.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        }).GeneratePdf();

        _logger.LogInformation("Invoice {InvoiceNo} generated in {Ms} ms", invoiceNo, (DateTime.UtcNow - started).TotalMilliseconds);
        return (pdf, $"{invoiceNo}.pdf");
    }

    private static IContainer Box(IContainer c) =>
        c.Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten5).Padding(12);

    private static string Money(decimal value) => value.ToString("#,##0", Vi) + " đ";
}
