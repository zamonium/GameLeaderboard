namespace GameLeaderboard.Infrastructure.DTOs.Leaderboard;

public class GetScoreResult
{
    public ScoreDetails? Score {get; private set;}

    public static GetScoreResult Ok(ScoreDetails score) => new() {Score = score};
}
