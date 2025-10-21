using ChatService.Application.Contracts;
using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;

namespace ChatService.Application.Services;

public class ChatAppService
{
    private readonly IMessageRepository _messages;
    private readonly ISentimentClient _sentiment;

    public ChatAppService(IMessageRepository messages, ISentimentClient sentiment)
    {
        _messages = messages;
        _sentiment = sentiment;
    }

    public async Task<MessageResponse> SendAsync(Guid userId, string nickname, SendMessageRequest request, CancellationToken ct = default)
    {
        var (label, score) = await _sentiment.AnalyzeAsync(request.Text, ct);
        var msg = new Message(userId, nickname, request.Text, label, score);
        await _messages.AddAsync(msg, ct);
        await _messages.SaveChangesAsync(ct);

        return new MessageResponse(msg.Id, msg.UserId, msg.Nickname, msg.Text, msg.SentAtUtc, msg.SentimentLabel, msg.SentimentScore);
    }

    public async Task<IReadOnlyList<MessageResponse>> GetRecentAsync(int take = 50, CancellationToken ct = default)
    {
        var list = await _messages.GetRecentAsync(take, ct);
        return list
            .OrderBy(m => m.SentAtUtc)
            .Select(m => new MessageResponse(m.Id, m.UserId, m.Nickname, m.Text, m.SentAtUtc, m.SentimentLabel, m.SentimentScore))
            .ToList();
    }
}