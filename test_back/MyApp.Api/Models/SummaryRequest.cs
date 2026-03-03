namespace MyApp.Api.Models;

/// <summary>
/// AI 요약 요청 모델
/// </summary>
public class SummaryRequest
{
    /// <summary>
    /// 요약할 텍스트 (필수)
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// 요약 언어 (기본값: ko)
    /// </summary>
    public string Language { get; init; } = "ko";

    /// <summary>
    /// 요약 최대 길이 (기본값: 200자)
    /// </summary>
    public int MaxLength { get; init; } = 200;
}
