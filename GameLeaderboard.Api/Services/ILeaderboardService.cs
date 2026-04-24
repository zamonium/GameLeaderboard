using GameLeaderboard.Api.DTOs.Leaderboard;

namespace GameLeaderboard.Api.Services;

public interface ILeaderboardService
{
    Task<GetScoreResult> GetScoreAsync(int id, CancellationToken ct);
    Task<List<ScoreSummary>> GetScoresAsync(int amount, CancellationToken ct);
    Task<SubmitScoreResult> SubmitScore(int userId, SubmitScoreRequest request, CancellationToken ct);
    //Task<DeleteScoreResult> DeleteScore(DeleteScoreRequest request, CancellationToken ct);
}
