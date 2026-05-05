namespace GameLeaderboard.Infrastructure.DTOs.Leaderboard;

public class SubmitScoreResult
{
    public ScoreDetails? Score {get; private set;}

    public static SubmitScoreResult Ok(ScoreDetails score) => new SubmitScoreResult() {Score = score};
}
