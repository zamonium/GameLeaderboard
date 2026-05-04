using System.ComponentModel.DataAnnotations;

namespace GameLeaderboard.Infrastructure.DTOs.Leaderboard;

public record SubmitScoreRequest(
    [Required] [Range(0, int.MaxValue)] int Amount,
    [Required] DateTime CreatedAt
);
