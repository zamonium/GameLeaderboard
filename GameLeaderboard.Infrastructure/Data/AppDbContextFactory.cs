using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GameLeaderboard.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<GameLeaderboardContext>
{
    public GameLeaderboardContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<GameLeaderboardContext>()
            .UseSqlite("DataSource=leaderboard_design.db")
            .Options;

        return new GameLeaderboardContext(options);
    }
}