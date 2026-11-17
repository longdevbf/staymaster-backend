using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace HotelManagement.API.Services.Ai;

/// <summary>
/// Google Gemini qua REST API (generateContent). Key tạo ở Google AI Studio (dạng "AIza...").
/// - Structured output: responseMimeType = application/json + responseSchema.
/// - Model mặc định gemini-2.5-flash, tắt "thinking" để giữ dưới mốc 5 giây của SRS 3a.
/// </summary>
public class GeminiChatModel : IChatModel
{
    public const string DefaultModel = "gemini-2.5-flash";
    public const string DefaultEndpoint = "https://generativelanguage.googleapis.com/v1beta";

    private readonly HttpClient _http;
    private readonly string _model;
    private readonly string _apiKey;
    private readonly string _endpoint;

    public GeminiChatModel(AiOptions options, HttpClient http)
    {
        _http = http;
        _http.Timeout = TimeSpan.FromSeconds(Math.Max(1, options.TimeoutSeconds) + 1);
        _model = string.IsNullOrWhiteSpace(options.Model) ? DefaultModel : options.Model;
        _apiKey = options.ApiKey ?? "";
        _endpoint = (string.IsNullOrWhiteSpace(options.Endpoint) ? DefaultEndpoint : options.Endpoint).TrimEnd('/');
    }

    public string Name => $"gemini:{_model}";

    public async Task<string> CompleteJsonAsync(string systemPrompt, IReadOnlyList<ChatTurn> turns,
        IReadOnlyDictionary<string, JsonElement> jsonSchema, CancellationToken ct)
    {
        var generationConfig = new JsonObject
        {
            ["responseMimeType"] = "application/json",
            ["responseSchema"] = ToGeminiSchema(JsonSerializer.SerializeToElement(jsonSchema)),
            ["maxOutputTokens"] = 1024,
        };
        // Gemini 2.5 Flash bật "thinking" mặc định; tắt đi cho câu trả lời chat ngắn và nhanh
        if (_model.StartsWith("gemini-2.5-flash", StringComparison.OrdinalIgnoreCase))
            generationConfig["thinkingConfig"] = new JsonObject { ["thinkingBudget"] = 0 };

        var body = new JsonObject
        {
            ["systemInstruction"] = new JsonObject { ["parts"] = new JsonArray(new JsonObject { ["text"] = systemPrompt }) },
            ["contents"] = new JsonArray(turns.Select(t => (JsonNode)new JsonObject
            {
                ["role"] = t.Role == "assistant" ? "model" : "user",
                ["parts"] = new JsonArray(new JsonObject { ["text"] = t.Content }),
            }).ToArray()),
            ["generationConfig"] = generationConfig,
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_endpoint}/models/{Uri.EscapeDataString(_model)}:generateContent")
        {
            Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json"),
        };
        request.Headers.Add("x-goog-api-key", _apiKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _http.SendAsync(request, ct);
        var json = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Gemini returned {(int)response.StatusCode}: {ErrorSummary(json)}");

        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            throw new InvalidOperationException($"Gemini returned no candidates ({ErrorSummary(json)})");

        var candidate = candidates[0];
        var finish = candidate.TryGetProperty("finishReason", out var f) ? f.GetString() : null;
        if (finish is "SAFETY" or "PROHIBITED_CONTENT" or "BLOCKLIST" or "RECITATION")
            throw new InvalidOperationException($"Gemini blocked the answer ({finish})");

        var text = candidate.TryGetProperty("content", out var content) && content.TryGetProperty("parts", out var parts)
            ? string.Concat(parts.EnumerateArray().Select(p => p.TryGetProperty("text", out var t) ? t.GetString() : ""))
            : "";
        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException($"Empty Gemini response (finish reason {finish})");
        return text;
    }

    /// <summary>
    /// responseSchema của Gemini dùng tập con OpenAPI: không có additionalProperties,
    /// và kiểu cho phép null viết là { "type": "integer", "nullable": true } thay vì ["integer", "null"].
    /// </summary>
    public static JsonNode ToGeminiSchema(JsonElement schema)
    {
        var result = new JsonObject();
        foreach (var prop in schema.EnumerateObject())
        {
            switch (prop.Name)
            {
                case "additionalProperties":
                    break;
                case "type" when prop.Value.ValueKind == JsonValueKind.Array:
                    var types = prop.Value.EnumerateArray().Select(t => t.GetString()).ToList();
                    result["type"] = types.First(t => t != "null");
                    if (types.Contains("null")) result["nullable"] = true;
                    break;
                case "properties":
                    var props = new JsonObject();
                    foreach (var p in prop.Value.EnumerateObject()) props[p.Name] = ToGeminiSchema(p.Value);
                    result["properties"] = props;
                    break;
                case "items":
                    result["items"] = ToGeminiSchema(prop.Value);
                    break;
                default:
                    result[prop.Name] = JsonNode.Parse(prop.Value.GetRawText());
                    break;
            }
        }
        return result;
    }

    /// <summary>Thông báo lỗi ngắn từ body lỗi của Google, không bao giờ chứa API key.</summary>
    private static string ErrorSummary(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("error", out var e))
            {
                var status = e.TryGetProperty("status", out var s) ? s.GetString() : "";
                var reasons = e.TryGetProperty("details", out var d) && d.ValueKind == JsonValueKind.Array
                    ? string.Join(",", d.EnumerateArray().Where(x => x.TryGetProperty("reason", out _)).Select(x => x.GetProperty("reason").GetString()))
                    : "";
                return $"{status} {reasons}".Trim();
            }
        }
        catch (JsonException) { }
        return json.Length > 120 ? json[..120] : json;
    }
}
