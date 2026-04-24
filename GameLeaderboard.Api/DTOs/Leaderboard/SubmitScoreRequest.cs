using System.ComponentModel.DataAnnotations;

namespace GameLeaderboard.Api.DTOs.Leaderboard;

public record SubmitScoreRequest(
    [Required] int Amount,
    [Required] DateTime CreatedAt
);
