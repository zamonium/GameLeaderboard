namespace GameLeaderboard.Infrastructure.DTOs.Leaderboard;

public class GetScoreResult
{
    public bool Success {get; private set;}
    public ScoreDetails? Score {get; private set;}
    public string? Error {get; private set;}

    public static GetScoreResult Ok(ScoreDetails score) => new() {Success = true, Score = score};
    public static GetScoreResult Fail(string error) => new() {Success = false, Error = error};
}
