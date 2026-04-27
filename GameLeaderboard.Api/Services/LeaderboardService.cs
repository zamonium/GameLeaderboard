using GameLeaderboard.Api.Data;
using GameLeaderboard.Api.DTOs;
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
        var score = await dbContext.Scores.FirstOrDefaultAsync(s => s.Id == id, ct);

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

    public async Task<GetScoresResult> GetAllScoresAsync(GetScoresRequest request, CancellationToken ct)
    {
        var query = dbContext.Scores
            .AsNoTracking()
            .AsQueryable();

        int totalCount = await query.CountAsync(ct);

        query = request.SortBy?.ToLower() switch
        {
            "amount" => request.Desc ? query.OrderByDescending(s => s.Amount) : query.OrderBy(s => s.Amount),
            "createdat" => request.Desc ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
            "username" => request.Desc ? query.OrderByDescending(s => s.User!.Username) : query.OrderBy(s => s.User!.Username),
            _ => query.OrderByDescending(s => s.Amount)
        };

        var scores = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ScoreSummary(s.User!.Username, s.Amount, s.CreatedAt))
            .ToListAsync(ct);

        var pagedResult = new PagedResult<ScoreSummary>
        {
            Items = scores,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };

        return GetScoresResult.Ok(pagedResult);
    }

    public async Task<GetScoresResult> GetUserScoresAsync(string username, GetScoresRequest request, CancellationToken ct)
    {
        var query = dbContext.Scores
            .AsNoTracking()
            .Where(s => s.User!.Username == username);
        
        int totalCount = await query.CountAsync(ct);

        query = request.SortBy?.ToLower() switch
        {
            "amount" => request.Desc ? query.OrderByDescending(s => s.Amount) : query.OrderBy(s => s.Amount),
            "createdat" => request.Desc ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
            _ => query.OrderByDescending(s => s.Amount)
        };

        var scores = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ScoreSummary(s.User!.Username, s.Amount, s.CreatedAt))
            .ToListAsync(ct);

        var pagedResult = new PagedResult<ScoreSummary>
        {
            Items = scores,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };

        return GetScoresResult.Ok(pagedResult);
    }

    public async Task<SubmitScoreResult> SubmitScore(int userId, SubmitScoreRequest request, CancellationToken ct)
    {
        if(userId <= 0)
        {
            return SubmitScoreResult.Fail("Wrong userId");    
        }

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
