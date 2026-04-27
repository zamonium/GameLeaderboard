namespace GameLeaderboard.Api.DTOs.Leaderboard;

public record ScoreSummary
(
    string Username,
    int Amount,
    DateTime CreatedAt
);
