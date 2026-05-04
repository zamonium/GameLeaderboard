namespace GameLeaderboard.Infrastructure.DTOs.Leaderboard;

public record ScoreDetails(
    int Id,
    int Amount,
    DateTime CreatedAt
);
