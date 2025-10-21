using System.Security.Claims;
using ChatService.API.Hubs;
using ChatService.Application.Contracts;
using ChatService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly ChatAppService _chat;
    private readonly IHubContext<ChatHub> _hub;

    public MessagesController(ChatAppService chat, IHubContext<ChatHub> hub)
    {
        _chat = chat;
        _hub = hub;
    }

    [HttpPost]
    public async Task<ActionResult<MessageResponse>> Send([FromBody] SendMessageRequest request, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var nickname = User.FindFirstValue("nickname") ?? "unknown";
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var resp = await _chat.SendAsync(userId, nickname, request, ct);
        await _hub.Clients.All.SendAsync("ReceiveMessage", resp, ct);
        return Ok(resp);
    }

    [HttpGet("recent")]
    public async Task<ActionResult<IReadOnlyList<MessageResponse>>> Recent([FromQuery] int take = 50, CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 200);
        var list = await _chat.GetRecentAsync(take, ct);
        return Ok(list);
    }
}