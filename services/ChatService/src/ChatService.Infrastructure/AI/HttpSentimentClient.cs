using System.Net.Http.Json;
using ChatService.Application.Interfaces;

namespace ChatService.Infrastructure.AI;

public class HttpSentimentClient : ISentimentClient
{
    private readonly HttpClient _http;

    public HttpSentimentClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<(string? label, float? score)> AnalyzeAsync(string text, CancellationToken ct = default)
    {
        try
        {
            var payload = new { text };
            using var resp = await _http.PostAsJsonAsync("analyze", payload, ct);
            resp.EnsureSuccessStatusCode();
            var data = await resp.Content.ReadFromJsonAsync<SentimentDto>(cancellationToken: ct);
            return (data?.label, data?.score);
        }
        catch
        {
            return (null, null);
        }
    }

    private sealed class SentimentDto
    {
        public string? label { get; set; }
        public float? score { get; set; }
    }
}