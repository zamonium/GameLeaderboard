using GameLeaderboard.Api.DTOs;
using GameLeaderboard.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameLeaderboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request, ct);
        if (!result.Success)
        {
            return Conflict(result.Error);
        }
        
        return Created();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);
        if (!result.Success)
        {
            return Unauthorized(result.Error);
        }

        return Ok(new LoginResponse(result.Token!));
    }
}
