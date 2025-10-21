namespace ChatService.Application.Interfaces;

public interface ISentimentClient
{
    // Döndürülen: (label, score) ör: ("positive", 0.94f)
    Task<(string? label, float? score)> AnalyzeAsync(string text, CancellationToken ct = default);
}