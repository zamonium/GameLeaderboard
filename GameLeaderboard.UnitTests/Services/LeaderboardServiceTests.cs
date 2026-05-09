using FluentAssertions;
using GameLeaderboard.Domain.Entities;
using GameLeaderboard.Infrastructure.DTOs.Leaderboard;
using GameLeaderboard.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace GameLeaderboard.UnitTests.Services;

public class LeaderboardServiceTests
{
    [Fact]
    public async Task GetScoreAsync_should_throw_when_score_is_missing()
    {
        await using var db = TestDbContextFactory.Create();
        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var act = () => service.GetScoreAsync(id: 999, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetScoreAsync_should_return_score_when_present()
    {
        await using var db = TestDbContextFactory.Create();
        db.Scores.Add(new Domain.Entities.Score{Amount = 100, CreatedAt = DateTime.UtcNow});
        await db.SaveChangesAsync();

        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var result = await service.GetScoreAsync(id: 1, CancellationToken.None);

        result.Score.Should().NotBeNull();
        result.Score.Amount.Should().Be(100);
    }

    [Fact]
    public async Task SubmitScore_shoul_persist_a_new_score()
    {
        await using var db = TestDbContextFactory.Create();
        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var request = new SubmitScoreRequest(Amount: 100, CreatedAt: DateTime.UtcNow);
        var result = await service.SubmitScore(userId: 1, request, CancellationToken.None);

        result.Score.Should().NotBeNull();
        db.Scores.Should().ContainSingle(s => s.Amount == 100 && s.UserId == 1);
    }

    [Fact]
    public async Task SubmitScore_should_throw_when_userId_is_invalid()
    {
        await using var db = TestDbContextFactory.Create();
        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var request = new SubmitScoreRequest(Amount: 100, CreatedAt: DateTime.UtcNow);
        var act = () => service.SubmitScore(userId: -1, request, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAllScoresAsync_should_return_paged_score_sorted_by_amount_desc_by_default()
    {
        await using var db = TestDbContextFactory.Create();
        var user = new User {Email = "test@example.com", Username = "test"};
        db.Users.Add(user);
        db.Scores.AddRange(
            new Score {Amount = 10, CreatedAt = DateTime.UtcNow, User = user},    
            new Score {Amount = 20, CreatedAt = DateTime.UtcNow, User = user},    
            new Score {Amount = 30, CreatedAt = DateTime.UtcNow, User = user}    
        );
        await db.SaveChangesAsync();

        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var result = await service.GetAllScoresAsync(new GetScoresRequest(), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.Items.Select(i => i.Amount).Should().ContainInOrder(30, 20, 10);
        result.Data.TotalCount.Should().Be(3);
    }
}
