using GameLeaderboard.Api.Models;

namespace GameLeaderboard.Api.DTOs.Leaderboard;

public class SubmitScoreResult
{
    public bool Success {get; private set;}
    public ScoreDetails? Score {get; private set;}
    public string? Error {get; private set;}

    public static SubmitScoreResult Ok(ScoreDetails score) => new SubmitScoreResult() {Success = true, Score = score};
    public static SubmitScoreResult Fail(string error) => new() {Success = false, Error = error};
}
