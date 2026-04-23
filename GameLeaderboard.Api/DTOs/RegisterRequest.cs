namespace GameLeaderboard.Api.DTOs;

public record RegisterRequest(
    string Username,
    string Email,
    string Password
);
