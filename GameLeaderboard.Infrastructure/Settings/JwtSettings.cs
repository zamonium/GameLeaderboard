namespace GameLeaderboard.Infrastructure.Settings;

public class JwtSettings
{
    public string Secret {get; set;} = string.Empty;
    public int ExpiryHours {get; set;}
}
