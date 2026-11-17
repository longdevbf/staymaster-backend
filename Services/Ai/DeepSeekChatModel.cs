using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace HotelManagement.API.Services.Ai;

/// <summary>
/// DeepSeek qua API chat/completions (định dạng tương thích OpenAI).
/// - JSON mode (response_format = json_object); schema được ghi vào system prompt vì API không nhận schema.
/// - Model mặc định deepseek-flash, tắt "thinking": khoảng 1,5 giây cho câu trả lời ngắn (SRS 3a).
/// </summary>
public class DeepSeekChatModel : IChatModel
{
    public const string DefaultModel = "deepseek-flash";
    public const string DefaultEndpoint = "https://api.deepseek.com";

    private readonly HttpClient _http;
    private readonly string _model;
    private readonly string _apiKey;
    private readonly string _endpoint;

    public DeepSeekChatModel(AiOptions options, HttpClient http)
    {
        _http = http;
        _http.Timeout = TimeSpan.FromSeconds(Math.Max(1, options.TimeoutSeconds) + 1);
        _model = string.IsNullOrWhiteSpace(options.Model) ? DefaultModel : options.Model;
        _apiKey = options.ApiKey ?? "";
        _endpoint = (string.IsNullOrWhiteSpace(options.Endpoint) ? DefaultEndpoint : options.Endpoint).TrimEnd('/');
    }

    public string Name => $"deepseek:{_model}";

    public async Task<string> CompleteJsonAsync(string systemPrompt, IReadOnlyList<ChatTurn> turns,
        IReadOnlyDictionary<string, JsonElement> jsonSchema, CancellationToken ct)
    {
        var system = systemPrompt +
                     "\n\nChỉ trả về một đối tượng JSON hợp lệ (json), không có chữ nào khác, đúng JSON Schema sau:\n" +
                     JsonSerializer.Serialize(jsonSchema);

        var messages = new JsonArray(new JsonObject { ["role"] = "system", ["content"] = system });
        foreach (var t in turns)
        {
            // Lượt cũ của trợ lý chỉ lưu phần chữ; gửi lại dưới dạng JSON cho khớp JSON mode,
            // nếu không DeepSeek hay trả nội dung rỗng ở lượt sau
            var content = t.Role == "assistant"
                ? JsonSerializer.Serialize(new { reply = t.Content, intent = "answer", roomIds = Array.Empty<int>(), forwardRoomId = (int?)null, questionSummary = "" })
                : t.Content;
            messages.Add(new JsonObject { ["role"] = t.Role == "assistant" ? "assistant" : "user", ["content"] = content });
        }

        var body = new JsonObject
        {
            ["model"] = _model,
            ["messages"] = messages,
            ["response_format"] = new JsonObject { ["type"] = "json_object" },
            ["thinking"] = new JsonObject { ["type"] = "disabled" },
            ["max_tokens"] = 1024,
            ["stream"] = false,
        };

        // JSON mode của DeepSeek thỉnh thoảng trả nội dung rỗng (tài liệu của DeepSeek có ghi): thử lại,
        // vẫn trong giới hạn thời gian chung của ChatService
        const int maxAttempts = 3;
        for (var attempt = 1; ; attempt++)
        {
            var text = await SendOnceAsync(body, ct);
            if (!string.IsNullOrWhiteSpace(text)) return text;
            if (attempt >= maxAttempts)
                throw new InvalidOperationException($"Empty DeepSeek response after {maxAttempts} attempts");
        }
    }

    /// <summary>Một lần gọi API; trả về null khi DeepSeek trả nội dung rỗng.</summary>
    private async Task<string?> SendOnceAsync(JsonObject body, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_endpoint}/chat/completions")
        {
            Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json"),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        using var response = await _http.SendAsync(request, ct);
        var json = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"DeepSeek returned {(int)response.StatusCode}: {ErrorSummary(json)}");

        using var doc = JsonDocument.Parse(json);
        var choice = doc.RootElement.GetProperty("choices")[0];
        var finish = choice.TryGetProperty("finish_reason", out var f) ? f.GetString() : null;
        if (finish is "content_filter")
            throw new InvalidOperationException("DeepSeek filtered the answer");
        if (finish is "length")
            throw new InvalidOperationException("DeepSeek answer was cut off (max_tokens)");

        return choice.GetProperty("message").TryGetProperty("content", out var c) ? c.GetString() : null;
    }

    /// <summary>Thông báo lỗi ngắn từ body lỗi, không bao giờ chứa API key.</summary>
    private static string ErrorSummary(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("error", out var e))
                return e.TryGetProperty("message", out var m) ? m.GetString() ?? "" : e.ToString();
        }
        catch (JsonException) { }
        return json.Length > 120 ? json[..120] : json;
    }
}
