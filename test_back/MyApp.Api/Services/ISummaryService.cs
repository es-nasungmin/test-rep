using MyApp.Api.Models;

namespace MyApp.Api.Services;

/// <summary>
/// AI 요약 서비스 인터페이스
/// </summary>
public interface ISummaryService
{
    /// <summary>
    /// 주어진 텍스트를 AI로 요약합니다.
    /// </summary>
    Task<SummaryResponse> SummarizeAsync(SummaryRequest request, CancellationToken cancellationToken = default);
}
