namespace ChatService.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Nickname { get; private set; } = string.Empty;
    public string Text { get; private set; } = string.Empty;
    public DateTimeOffset SentAtUtc { get; private set; } = DateTimeOffset.UtcNow;

    public string? SentimentLabel { get; private set; }
    public float? SentimentScore { get; private set; }

    private Message() { } // EF

    public Message(Guid userId, string nickname, string text, string? label, float? score)
    {
        if (userId == Guid.Empty) throw new ArgumentException("userId required");
        if (string.IsNullOrWhiteSpace(nickname)) throw new ArgumentException("nickname required");
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("text required");

        UserId = userId;
        Nickname = nickname.Trim();
        Text = text.Trim();
        SentimentLabel = label;
        SentimentScore = score;
        SentAtUtc = DateTimeOffset.UtcNow;
    }
}