using GameLeaderboard.Api.DTOs;

namespace GameLeaderboard.Api.Services;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest request);
    Task<LoginResult> LoginAsync(LoginRequest request);
}
