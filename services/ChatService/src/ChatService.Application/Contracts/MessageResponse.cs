namespace ChatService.Application.Contracts;

public record MessageResponse(
    Guid Id,
    Guid UserId,
    string Nickname,
    string Text,
    DateTimeOffset SentAtUtc,
    string? SentimentLabel,
    float? SentimentScore
);