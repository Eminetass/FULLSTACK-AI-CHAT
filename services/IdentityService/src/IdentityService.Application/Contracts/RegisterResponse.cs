namespace IdentityService.Application.Contracts;

public record RegisterResponse(Guid UserId, string Nickname, string Token);