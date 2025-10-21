using IdentityService.Application.Interfaces;
using IdentityService.Application.Services;
using IdentityService.Infrastructure.Auth;
using IdentityService.Infrastructure.Data;
using IdentityService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string sqliteConn,
        string issuer,
        string audience,
        string jwtSecret)
    {
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(sqliteConn));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<AuthService>();
        services.AddSingleton<ITokenService>(_ => new TokenService(issuer, audience, jwtSecret));
        return services;
    }
}