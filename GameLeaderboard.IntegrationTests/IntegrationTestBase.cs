using GameLeaderboard.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GameLeaderboard.IntegrationTests;

public class IntegrationTestBase : IClassFixture<GameLeaderboardWebAppFactory>, IAsyncLifetime
{
    protected readonly GameLeaderboardWebAppFactory Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(GameLeaderboardWebAppFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    //Reset DB state between tests
    public async Task InitializeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GameLeaderboardContext>();
        db.Scores.RemoveRange(db.Scores);
        db.Users.RemoveRange(db.Users);
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
