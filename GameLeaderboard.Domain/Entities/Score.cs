namespace GameLeaderboard.Domain.Entities;

public class Score
{
    public int Id {get; set;}
    public required int Amount {get; set;}
    public required DateTime CreatedAt {get; set;}
    public int UserId {get; set;}
    public User? User {get; set;}
}
