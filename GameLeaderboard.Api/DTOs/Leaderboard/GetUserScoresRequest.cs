using System.ComponentModel.DataAnnotations;

namespace GameLeaderboard.Api.DTOs.Leaderboard;

public record GetUserScoresRequest
(
    [Range(1, int.MaxValue)] int Page = 1,
    [Range(1, 100)] int PageSize = 10,
    bool Desc = true,
    string? SortBy = null
);