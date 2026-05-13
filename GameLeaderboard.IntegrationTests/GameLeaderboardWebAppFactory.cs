using GameLeaderboard.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GameLeaderboard.IntegrationTests;

public class GameLeaderboardWebAppFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "test_long_enough_secret_for_hs256_!!!!!!!",
                ["Jwt:ExpiryHours"] = "1"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddDbContext<GameLeaderboardContext>(options =>
                options.UseSqlite(connection));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<GameLeaderboardContext>()
            .Database.EnsureCreated();
        return host;
    }

    protected override void Dispose(bool disposing)
    {
        connection?.Dispose();
        base.Dispose(disposing);
    }
}
