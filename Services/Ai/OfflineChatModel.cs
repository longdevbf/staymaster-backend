using System.Text.Json;
using System.Text.RegularExpressions;

namespace HotelManagement.API.Services.Ai;

/// <summary>
/// Bản chạy không cần API key, dùng cho máy dev và demo: nhận diện ý định bằng từ khoá và
/// gợi ý phòng theo thành phố/giá trong danh mục. Không thay được mô hình thật.
/// </summary>
public class OfflineChatModel : IChatModel
{
    public string Name => "offline";

    public Task<string> CompleteJsonAsync(string systemPrompt, IReadOnlyList<ChatTurn> turns,
        IReadOnlyDictionary<string, JsonElement> jsonSchema, CancellationToken ct)
    {
        var question = RecommendationService.Normalize(turns.LastOrDefault(t => t.Role == "user")?.Content);
        var catalog = ChatService.ParseCatalog(systemPrompt);
        var contextRoomId = ChatService.ParseContextRoomId(systemPrompt);

        string intent = "answer";
        int? forwardRoomId = null;
        string reply;
        var roomIds = new List<int>();

        if (Regex.IsMatch(question, @"\b(admin|quan tri|khieu nai|hoan tien|tai khoan bi)\b"))
        {
            intent = "forward_admin";
            reply = "Mình đã chuyển câu hỏi này cho bộ phận hỗ trợ StayMaster, họ sẽ trả lời bạn ngay trong khung chat này, bạn nhận thông báo khi có trả lời.";
        }
        else if (Regex.IsMatch(question, @"(chu nha|hoi chu|thu cung|(mang|nuoi|cho) (cho|meo)|nhan phong som|tra phong muon|hoa don cong ty|dau (o to|xe)|cho dau)"))
        {
            intent = "forward_partner";
            forwardRoomId = contextRoomId ?? catalog.FirstOrDefault()?.Id;
            reply = "Câu hỏi này chủ nhà sẽ trả lời chính xác nhất. Mình đã chuyển tới chủ nhà, chủ nhà sẽ trả lời bạn ngay trong khung chat này, bạn nhận thông báo khi có trả lời.";
        }
        else if (Regex.IsMatch(question, @"(huy|hoan coc)"))
        {
            reply = "Bạn có thể tự huỷ đơn khi đơn còn ở trạng thái \"Chờ duyệt\" trong mục Đơn đặt phòng của tôi. Đơn đã xác nhận thì cần liên hệ chủ nhà.";
        }
        else if (Regex.IsMatch(question, @"(thanh toan|chuyen khoan|qr)"))
        {
            reply = "StayMaster nhận chuyển khoản qua mã QR ở bước thanh toán. Đơn được xác nhận tự động khi hệ thống nhận đủ tiền.";
        }
        else
        {
            var matches = catalog
                .Where(r => question.Contains(RecommendationService.Normalize(r.City)) ||
                            question.Contains(RecommendationService.Normalize(r.RoomType)))
                .ToList();
            var budget = ParseBudget(question);
            if (budget is not null) matches = (matches.Count > 0 ? matches : catalog).Where(r => r.Price <= budget).ToList();
            roomIds = (matches.Count > 0 ? matches : catalog).OrderByDescending(r => r.Rating ?? 0).Take(3).Select(r => r.Id).ToList();
            reply = matches.Count > 0
                ? "Đây là vài phòng phù hợp với yêu cầu của bạn:"
                : "Mình chưa tìm được phòng khớp hoàn toàn, bạn tham khảo các phòng được đánh giá cao này nhé:";
        }

        var json = JsonSerializer.Serialize(new
        {
            reply,
            intent,
            roomIds,
            forwardRoomId,
            questionSummary = turns.LastOrDefault(t => t.Role == "user")?.Content ?? "",
        });
        return Task.FromResult(json);
    }

    /// <summary>"duoi 1 trieu", "500k", "800.000" → số tiền.</summary>
    private static decimal? ParseBudget(string q)
    {
        var m = Regex.Match(q, @"(\d+(?:[.,]\d+)?)\s*(trieu|tr|k|nghin|ngan)?");
        if (!m.Success) return null;
        var number = decimal.Parse(m.Groups[1].Value.Replace(".", "").Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
        return m.Groups[2].Value switch
        {
            "trieu" or "tr" => number * 1_000_000,
            "k" or "nghin" or "ngan" => number * 1_000,
            _ => number >= 10_000 ? number : null,
        };
    }
}
