namespace GameLeaderboard.Infrastructure.DTOs.Leaderboard;

public record ScoreSummary
(
    string Username,
    int Amount,
    DateTime CreatedAt
);
