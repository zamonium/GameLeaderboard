namespace GameLeaderboard.Api.DTOs;

public class LoginResult
{
    public bool Success {get; private set;}
    public string? Token {get; private set;}
    public string? Error {get; private set;}

    public static LoginResult Ok(string token) => new() {Success = true, Token = token};
    public static LoginResult Fail(string error) => new() {Success = false, Error = error};
}
