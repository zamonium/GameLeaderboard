using GameLeaderboard.Infrastructure.DTOs.Auth;

namespace GameLeaderboard.Infrastructure.Services;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct);
}
