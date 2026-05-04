using System.ComponentModel.DataAnnotations;

namespace GameLeaderboard.Infrastructure.DTOs.Auth;

public record RegisterRequest(
    [Required] [StringLength(50)] string Username,
    [Required] [StringLength(50)] [EmailAddress] string Email,
    [Required] [StringLength(50)] string Password
);
