using GameLeaderboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLeaderboard.Infrastructure.Data;

public class GameLeaderboardContext : DbContext
{
    public GameLeaderboardContext(DbContextOptions<GameLeaderboardContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Score> Scores => Set<Score>();
}
