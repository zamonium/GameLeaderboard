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

        [HttpGet("scores")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllScores([FromQuery] GetAllScoresRequest request, CancellationToken ct)
        {
            var result = await leaderboardService.GetAllScoresAsync(request, ct);
            if (!result.Success)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Data);
        }

        [HttpGet("my-scores")]
        public async Task<IActionResult> GetUserScores([FromQuery] GetUserScoresRequest request, CancellationToken ct)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if(username == null)
            {
                return Unauthorized();    
            }

            var result = await leaderboardService.GetUserScoresAsync(username, request, ct);
            if (!result.Success)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Data);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitScore(SubmitScoreRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            var result = await leaderboardService.SubmitScore(userId, request, ct);
            if (!result.Success)
            {
                return BadRequest(result.Error);
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
