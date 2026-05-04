using System.Security.Claims;
using GameLeaderboard.Infrastructure.DTOs.Leaderboard;
using GameLeaderboard.Infrastructure.Services;
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
        [AllowAnonymous]
        public async Task<IActionResult> GetScore([FromRoute] int id, CancellationToken ct)
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
        public async Task<IActionResult> GetAllScores([FromQuery] GetScoresRequest request, CancellationToken ct)
        {
            var result = await leaderboardService.GetAllScoresAsync(request, ct);
            if (!result.Success)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Data);
        }

        [HttpGet("my-scores")]
        public async Task<IActionResult> GetUserScores([FromQuery] GetScoresRequest request, CancellationToken ct)
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
        public async Task<IActionResult> SubmitScore([FromBody] SubmitScoreRequest request, CancellationToken ct)
        {
            var userId = TryGetCurrentUserId();
            if(userId is null)
            {
                return Unauthorized("User identity could not be resolved");
            }

            var result = await leaderboardService.SubmitScore(userId.Value, request, ct);
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

        private int? TryGetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if(claim == null || !int.TryParse(claim.Value, out var id))
            {
                return null;
            }
            return id;
        }
    }
}
