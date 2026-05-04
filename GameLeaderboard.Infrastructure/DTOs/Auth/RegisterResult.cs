namespace GameLeaderboard.Infrastructure.DTOs.Auth;

public class RegisterResult
{
    public bool Success {get; private set;}
    public string? Error {get; private set;}

    public static RegisterResult Ok() => new() {Success = true};
    public static RegisterResult Fail(string error) => new() {Success = false, Error = error};
}
