namespace GameLeaderboard.Infrastructure.DTOs.Leaderboard;

public class GetScoresResult
{
    public bool Success {get; private set;}
    public PagedResult<ScoreSummary>? Data {get; private set;}
    public string? Error {get; private set;}

    public static GetScoresResult Ok(PagedResult<ScoreSummary> data) => new() {Success = true, Data = data};
    public static GetScoresResult Fail(string error) => new() {Success = false, Error = error};
}
