namespace GameLeaderboard.Domain.Entities;

public class User
{
    public int Id {get; set;}
    public required string Email {get; set;}
    public required string Username {get; set;}
    public string PasswordHash {get; set;} = string.Empty;
}
