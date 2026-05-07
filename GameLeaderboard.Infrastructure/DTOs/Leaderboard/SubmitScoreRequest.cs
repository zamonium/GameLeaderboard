namespace GameLeaderboard.Infrastructure.DTOs.Leaderboard;

public record SubmitScoreRequest(
    int Amount,
    DateTime CreatedAt
);
