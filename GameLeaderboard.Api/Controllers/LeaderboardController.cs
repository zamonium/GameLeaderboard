using System.Security.Claims;
using GameLeaderboard.Api.DTOs.Leaderboard;
using GameLeaderboard.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLeaderboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService leaderboardService;

        const string GetScoreEndpointName = "GetScore";

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            this.leaderboardService = leaderboardService;
        }

        [HttpGet("score/{id}", Name = GetScoreEndpointName)]
        public async Task<IActionResult> GetScore(int id, CancellationToken ct)
        {
            var result = await leaderboardService.GetScoreAsync(id, ct);
            if (!result.Success)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Score);
        }

        [HttpGet("{amount}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetScores(int amount, CancellationToken ct)
        {
            var result = await leaderboardService.GetScoresAsync(amount, ct);
            return Ok(result);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitScore(SubmitScoreRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            var result = await leaderboardService.SubmitScore(userId, request, ct);
            if (!result.Success)
            {
                //return ???
            }

            return CreatedAtAction(
                GetScoreEndpointName, 
                new {Id = result.Score!.Id}, 
                result.Score
            );
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim!.Value);
        }
    }
}
