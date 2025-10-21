using IdentityService.Application.Contracts;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Services;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;

    public AuthService(IUserRepository users, ITokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var existing = await _users.GetByNicknameAsync(request.Nickname, ct);
        if (existing is not null)
        {
            var existingToken = _tokens.CreateToken(existing);
            return new RegisterResponse(existing.Id, existing.Nickname, existingToken);
        }

        var user = new User(request.Nickname);
        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        var token = _tokens.CreateToken(user);
        return new RegisterResponse(user.Id, user.Nickname, token);
    }
}