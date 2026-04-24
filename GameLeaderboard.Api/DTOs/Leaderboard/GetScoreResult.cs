namespace GameLeaderboard.Api.DTOs.Leaderboard;

public class GetScoreResult
{
    public bool Success {get; set;}
    public ScoreDetails? Score {get; set;}
    public string? Error {get; set;}

    public static GetScoreResult Ok(ScoreDetails score) => new() {Success = true, Score = score};
    public static GetScoreResult Fail(string error) => new() {Success = false, Error = error};
}
