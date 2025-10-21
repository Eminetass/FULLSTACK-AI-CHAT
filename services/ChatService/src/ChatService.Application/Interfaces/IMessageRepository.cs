using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces;

public interface IMessageRepository
{
    Task AddAsync(Message message, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Message>> GetRecentAsync(int take = 50, CancellationToken ct = default);
}