namespace GameLeaderboard.Infrastructure.DTOs.Leaderboard;

public record GetScoresRequest
(
    int Page = 1,
    int PageSize = 10,
    bool Desc = true,
    string? SortBy = null
);
