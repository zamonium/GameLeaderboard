using System.ComponentModel.DataAnnotations;

namespace GameLeaderboard.Api.DTOs.Auth;

public record RegisterRequest(
    [Required] [StringLength(50)] string Username,
    [Required] [StringLength(50)] string Email,
    [Required] [StringLength(50)] string Password
);
