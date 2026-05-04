using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GameLeaderboard.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<GameLeaderboardContext>
{
    public GameLeaderboardContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config.GetConnectionString("GameLeaderboard");

        var options = new DbContextOptionsBuilder<GameLeaderboardContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new GameLeaderboardContext(options);
    }
}