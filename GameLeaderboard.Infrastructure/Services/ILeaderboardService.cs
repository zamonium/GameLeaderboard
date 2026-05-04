using GameLeaderboard.Infrastructure.DTOs.Leaderboard;

namespace GameLeaderboard.Infrastructure.Services;

public interface ILeaderboardService
{
    Task<GetScoreResult> GetScoreAsync(int id, CancellationToken ct);
    Task<GetScoresResult> GetAllScoresAsync(GetScoresRequest request, CancellationToken ct);
    Task<GetScoresResult> GetUserScoresAsync(string username, GetScoresRequest request, CancellationToken ct);
    Task<SubmitScoreResult> SubmitScore(int userId, SubmitScoreRequest request, CancellationToken ct);
    //Task<DeleteScoreResult> DeleteScore(DeleteScoreRequest request, CancellationToken ct);
}
