using GameLeaderboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameLeaderboard.UnitTests;

public static class TestDbContextFactory
{
    public static GameLeaderboardContext Create()
    {
        var options = new DbContextOptionsBuilder<GameLeaderboardContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new GameLeaderboardContext(options);
    }
}
