using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    // Şimdilik metot gerekmiyor; Controller üzerinden broadcast yapıyoruz.
}