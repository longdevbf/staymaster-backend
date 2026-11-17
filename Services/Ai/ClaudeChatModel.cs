using System.Text.Json;
using Anthropic;
using Anthropic.Models.Beta;
using Anthropic.Models.Beta.Messages;

namespace HotelManagement.API.Services.Ai;

/// <summary>
/// Claude qua SDK chính thức (gói NuGet "Anthropic").
/// - Structured outputs: câu trả lời luôn là JSON đúng schema.
/// - Effort "low": câu trả lời chat ngắn, giữ dưới mốc 5 giây của SRS 3a.
/// - Server-side fallbacks ("default"): nếu bộ lọc an toàn từ chối, API tự chuyển sang mô hình dự phòng.
/// </summary>
public class ClaudeChatModel : IChatModel
{
    public const string DefaultModel = "claude-opus-5-5";

    private readonly AnthropicClient _client;
    private readonly string _model;

    public ClaudeChatModel(AiOptions options)
    {
        _model = string.IsNullOrWhiteSpace(options.Model) ? DefaultModel : options.Model;
        _client = new AnthropicClient
        {
            ApiKey = options.ApiKey,
            Timeout = TimeSpan.FromSeconds(Math.Max(1, options.TimeoutSeconds)),
            MaxRetries = 0, // đã có giới hạn 5 giây; không thử lại để không vượt mốc
        };
    }

    public string Name => $"anthropic:{_model}";

    public async Task<string> CompleteJsonAsync(string systemPrompt, IReadOnlyList<ChatTurn> turns,
        IReadOnlyDictionary<string, JsonElement> jsonSchema, CancellationToken ct)
    {
        var messages = new List<BetaMessageParam>();
        foreach (var turn in turns)
        {
            messages.Add(turn.Role == "assistant"
                ? new BetaAssistantMessageParam { Content = turn.Content }
                : new BetaUserMessageParam { Content = turn.Content });
        }

        var response = await _client.Beta.Messages.Create(new MessageCreateParams
        {
            Model = _model,
            MaxTokens = 2048,
            Betas = [AnthropicBeta.ServerSideFallback2026_07_01],
            Fallbacks = new Default(),
            System = systemPrompt,
            OutputConfig = new BetaOutputConfig
            {
                Effort = Effort.Low,
                Format = new BetaJsonOutputFormat { Schema = jsonSchema },
            },
            Messages = messages,
        }, ct);

        if (response.StopReason == "refusal")
            throw new InvalidOperationException("Model declined the request");

        var text = string.Concat(response.Content
            .Select(b => b.TryPickText(out var t) ? t.Text : ""));
        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException($"Empty model response (stop reason {response.StopReason})");
        return text;
    }
}
