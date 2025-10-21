using ChatService.Application.Interfaces;
using ChatService.Application.Services;
using ChatService.Infrastructure.AI;
using ChatService.Infrastructure.Data;
using ChatService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddChatInfrastructure(
        this IServiceCollection services,
        string sqliteConn,
        string aiBaseUrl)
    {
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(sqliteConn));
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<ChatAppService>();

        services.AddHttpClient<ISentimentClient, HttpSentimentClient>(client =>
        {
            client.BaseAddress = new Uri(aiBaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        return services;
    }
}