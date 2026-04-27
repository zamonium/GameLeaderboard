using GameLeaderboard.Api.DTOs.Auth;

namespace GameLeaderboard.Api.Services;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct);
}
