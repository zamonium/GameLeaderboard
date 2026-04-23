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
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        if (!result.Success)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        if (!result.Success)
        {
            return Unauthorized(result.Error);
        }

        return Ok(new LoginResponse(result.Token!));
    }
}
