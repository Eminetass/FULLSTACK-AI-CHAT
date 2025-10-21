using System.Security.Claims;
using IdentityService.Application.Contracts;
using IdentityService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    public AuthController(AuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var resp = await _auth.RegisterAsync(request, ct);
        return Ok(resp);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        // Bazı hostlarda "sub" ClaimTypes.NameIdentifier'a maplenir.
        // Önce NameIdentifier'ı, yoksa "sub" claim'ini oku.
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");
        var nickname = User.FindFirstValue("nickname");
        return Ok(new { userId, nickname });
    }
}