namespace HotelManagement.API.Services.Ai;

public record ChatTurn(string Role, string Content);

/// <summary>
/// Mô hình ngôn ngữ đứng sau chatbot. Mỗi nhà cung cấp là một lớp riêng chọn qua <c>Ai:Provider</c>,
/// nên đổi hoặc nâng cấp mô hình không đụng tới phần nghiệp vụ (SRS 3e).
/// </summary>
public interface IChatModel
{
    /// <summary>Tên hiển thị trong log, ví dụ "anthropic:claude-opus-5-5".</summary>
    string Name { get; }

    /// <summary>Trả về một chuỗi JSON khớp <paramref name="jsonSchema"/>.</summary>
    Task<string> CompleteJsonAsync(string systemPrompt, IReadOnlyList<ChatTurn> turns,
        IReadOnlyDictionary<string, System.Text.Json.JsonElement> jsonSchema, CancellationToken ct);
}

public class AiOptions
{
    /// <summary>"anthropic", "gemini", "deepseek", "offline", hoặc rỗng (tắt chatbot, chỉ trả lời thay thế).</summary>
    public string? Provider { get; set; }
    public string? Model { get; set; }
    public string? ApiKey { get; set; }
    /// <summary>Base URL của API cho Gemini / DeepSeek (để trống = mặc định của nhà cung cấp).</summary>
    public string? Endpoint { get; set; }
    public int TimeoutSeconds { get; set; } = 5;
}

/// <summary>Chưa cấu hình nhà cung cấp: luôn báo không khả dụng để ChatService trả lời thay thế.</summary>
public class DisabledChatModel : IChatModel
{
    public string Name => "disabled";

    public Task<string> CompleteJsonAsync(string systemPrompt, IReadOnlyList<ChatTurn> turns,
        IReadOnlyDictionary<string, System.Text.Json.JsonElement> jsonSchema, CancellationToken ct) =>
        throw new InvalidOperationException("AI provider is not configured (Ai:Provider)");
}
