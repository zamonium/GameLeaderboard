using System.ComponentModel.DataAnnotations;

namespace GameLeaderboard.Api.DTOs;

public record LoginRequest(
    [Required] [StringLength(50)] string Email,
    [Required] [StringLength(50)] string Password
);
