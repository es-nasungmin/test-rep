using MyApp.Api.Models;
using OpenAI.Chat;

namespace MyApp.Api.Services;

/// <summary>
/// OpenAI GPT를 사용한 AI 요약 서비스 구현체
/// </summary>
public class SummaryService : ISummaryService
{
    private readonly ChatClient _chatClient;
    private readonly string _model;
    private readonly ILogger<SummaryService> _logger;

    public SummaryService(IConfiguration configuration, ILogger<SummaryService> logger)
    {
        _logger = logger;
        _model = configuration["OpenAI:Model"] ?? "gpt-4o-mini";

        var apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI:ApiKey가 설정되지 않았습니다.");

        _chatClient = new ChatClient(_model, apiKey);
    }

    public async Task<SummaryResponse> SummarizeAsync(SummaryRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("요약 요청 시작. 원본 길이: {Length}자", request.Text.Length);

        var systemPrompt =
            $"You are a professional text summarizer. " +
            $"Summarize the given text concisely in {request.Language} language. " +
            $"The summary must be within {request.MaxLength} characters. " +
            $"Return only the summary text without any additional explanation.";

        var userPrompt = $"다음 텍스트를 요약해 주세요:\n\n{request.Text}";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };

        var completion = await _chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);

        var summary = completion.Value.Content[0].Text;

        _logger.LogInformation("요약 완료. 요약 길이: {Length}자", summary.Length);

        return new SummaryResponse
        {
            Summary = summary,
            OriginalLength = request.Text.Length,
            SummaryLength = summary.Length,
            Model = _model,
            ProcessedAt = DateTime.UtcNow
        };
    }
}
