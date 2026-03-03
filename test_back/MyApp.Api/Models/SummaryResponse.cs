namespace MyApp.Api.Models;

/// <summary>
/// AI 요약 응답 모델
/// </summary>
public class SummaryResponse
{
    /// <summary>
    /// 요약된 텍스트
    /// </summary>
    public required string Summary { get; init; }

    /// <summary>
    /// 원본 텍스트 길이
    /// </summary>
    public int OriginalLength { get; init; }

    /// <summary>
    /// 요약 텍스트 길이
    /// </summary>
    public int SummaryLength { get; init; }

    /// <summary>
    /// 사용된 AI 모델
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// 처리 시각 (UTC)
    /// </summary>
    public DateTime ProcessedAt { get; init; } = DateTime.UtcNow;
}
