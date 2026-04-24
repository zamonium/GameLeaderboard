using GameLeaderboard.Api.Data;
using GameLeaderboard.Api.DTOs.Leaderboard;
using GameLeaderboard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLeaderboard.Api.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly GameLeaderboardContext dbContext;

    public LeaderboardService(GameLeaderboardContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<GetScoreResult> GetScoreAsync(int id, CancellationToken ct)
    {
        var score = await dbContext.Scores.FirstOrDefaultAsync(s => s.Id == id);

        if(score == null)
        {
            return GetScoreResult.Fail($"No score found with id {id}");
        }

        var scoreDetails = new ScoreDetails(
            score.Id,
            score.Amount,
            score.CreatedAt
        );

        return GetScoreResult.Ok(scoreDetails);
    }

    public async Task<List<ScoreSummary>> GetScoresAsync(int amount, CancellationToken ct)
    {
        var scores = await dbContext.Scores
                                        .Include(s => s.User)
                                        .OrderByDescending(s => s.Amount)
                                        .Take(amount)
                                        .Select(s => new ScoreSummary(s.User!.Username, s.Amount, s.CreatedAt))
                                        .AsNoTracking()
                                        .ToListAsync(ct);

        return scores;
        //should return a getScoreResult (instead of list)
        //then should probably add pagination
    }

    public async Task<SubmitScoreResult> SubmitScore(int userId, SubmitScoreRequest request, CancellationToken ct)
    {
        var score = new Score
        {
            UserId = userId,
            Amount = request.Amount,
            CreatedAt = request.CreatedAt
        };

        dbContext.Scores.Add(score);
        await dbContext.SaveChangesAsync(ct);

        ScoreDetails scoreDto = new(
            score.Id,
            score.Amount,
            score.CreatedAt
        );

        return SubmitScoreResult.Ok(scoreDto);
    }
}
