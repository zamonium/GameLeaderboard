using GameLeaderboard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLeaderboard.Api.Data;

public class GameLeaderboardContext : DbContext
{
    public GameLeaderboardContext(DbContextOptions<GameLeaderboardContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Score> Scores => Set<Score>();
}
