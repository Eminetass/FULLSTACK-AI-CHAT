using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using ChatService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _db;
    public MessageRepository(AppDbContext db) => _db = db;

    public Task AddAsync(Message message, CancellationToken ct = default)
        => _db.Messages.AddAsync(message, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task<IReadOnlyList<Message>> GetRecentAsync(int take = 50, CancellationToken ct = default)
        => await _db.Messages
            .OrderByDescending(m => m.SentAtUtc)
            .Take(take)
            .AsNoTracking()
            .ToListAsync(ct);
}