using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByNicknameAsync(string nickname, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Nickname == nickname, ct);

    public Task AddAsync(User user, CancellationToken ct = default)
        => _db.Users.AddAsync(user, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}