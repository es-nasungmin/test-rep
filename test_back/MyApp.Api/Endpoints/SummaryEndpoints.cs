using MyApp.Api.Models;
using MyApp.Api.Services;

namespace MyApp.Api.Endpoints;

/// <summary>
/// AI 요약 관련 엔드포인트 등록
/// </summary>
public static class SummaryEndpoints
{
    public static IEndpointRouteBuilder MapSummaryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/summary")
            .WithTags("Summary");

        group.MapPost("/", SummarizeAsync)
            .WithName("Summarize")
            .WithSummary("텍스트 AI 요약")
            .WithDescription("JSON으로 텍스트를 받아 OpenAI GPT로 요약한 결과를 반환합니다.");

        return app;
    }

    private static async Task<IResult> SummarizeAsync(
        SummaryRequest request,
        ISummaryService summaryService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return Results.BadRequest(new { error = "text 필드는 필수입니다." });

        if (request.Text.Length > 10_000)
            return Results.BadRequest(new { error = "text는 최대 10,000자까지 허용됩니다." });

        var response = await summaryService.SummarizeAsync(request, cancellationToken);
        return Results.Ok(response);
    }
}
