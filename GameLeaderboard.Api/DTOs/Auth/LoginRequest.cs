using System.ComponentModel.DataAnnotations;

namespace GameLeaderboard.Api.DTOs.Auth;

public record LoginRequest(
    [Required] [StringLength(50)] [EmailAddress] string Email,
    [Required] [StringLength(50)] string Password
);
