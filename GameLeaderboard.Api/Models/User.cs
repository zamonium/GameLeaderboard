using Microsoft.AspNetCore.Identity;

namespace GameLeaderboard.Api.Models;

public class User
{
    public int Id {get; set;}
    public required string Email {get; set;}
    public required string Username {get; set;}
    public string PasswordHash {get; set;} = string.Empty;

    public void SetPassword(string password, IPasswordHasher<User> passwordHasher)
    {
        PasswordHash = passwordHasher.HashPassword(this, password);
    }
}
