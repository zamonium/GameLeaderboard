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
        var score = new Score{Amount = 100, CreatedAt = DateTime.UtcNow};
        db.Scores.Add(score);
        await db.SaveChangesAsync();

        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var result = await service.GetScoreAsync(id: score.Id, CancellationToken.None);

        result.Score.Should().NotBeNull();
        result.Score.Amount.Should().Be(100);
    }

    [Fact]
    public async Task SubmitScore_should_persist_a_new_score()
    {
        await using var db = TestDbContextFactory.Create();
        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var request = new SubmitScoreRequest(Amount: 100, CreatedAt: DateTime.UtcNow);
        var result = await service.SubmitScore(userId: 1, request, CancellationToken.None);

        result.Score.Should().NotBeNull();
        db.Scores.Should().ContainSingle(s => s.Amount == 100 && s.UserId == 1);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task SubmitScore_should_throw_when_userId_is_invalid(int userId)
    {
        await using var db = TestDbContextFactory.Create();
        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var request = new SubmitScoreRequest(Amount: 100, CreatedAt: DateTime.UtcNow);
        var act = () => service.SubmitScore(userId: userId, request, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAllScoresAsync_should_return_empty_list_when_database_is_empty()
    {
        await using var db = TestDbContextFactory.Create();
        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var result = await service.GetAllScoresAsync(new GetScoresRequest(), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.TotalCount.Should().Be(0);
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

    [Fact]
    public async Task GetAllScoresAsync_should_return_paged_score_sorted_by_created_at_desc()
    {
        await using var db = TestDbContextFactory.Create();
        var user = new User {Email = "test@example.com", Username = "test"};
        db.Users.Add(user);
        db.Scores.AddRange(
            new Score {Amount = 10, CreatedAt = new DateTime(2026, 1, 1, 12, 0, 0), User = user},    
            new Score {Amount = 20, CreatedAt = new DateTime(2026, 1, 2, 12, 0, 0), User = user},    
            new Score {Amount = 30, CreatedAt = new DateTime(2026, 1, 3, 12, 0, 0), User = user}    
        );
        await db.SaveChangesAsync();

        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var request = new GetScoresRequest { SortBy = "CreatedAt", Desc = true};
        var result = await service.GetAllScoresAsync(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.Items.Select(i => i.Amount).Should().ContainInOrder(30, 20, 10);
        result.Data.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetAllScoresAsync_should_return_paged_score_when_pagination_parameter_specified()
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

        var request = new GetScoresRequest { Page = 2, PageSize = 2};
        var result = await service.GetAllScoresAsync(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.Items.Select(i => i.Amount).Should().ContainInOrder(10);
        result.Data.TotalCount.Should().Be(3);
        result.Data.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task GetUserScoresAsync_should_return_user_score()
    {
        await using var db = TestDbContextFactory.Create();
        var user = new User {Email = "test@example.com", Username = "test"};
        db.Users.Add(user);
        db.Scores.AddRange(
            new Score {Amount = 10, CreatedAt = DateTime.UtcNow, User = user},
            new Score {Amount = 20, CreatedAt = DateTime.UtcNow, User = user}
        );
        await db.SaveChangesAsync();

        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var result = await service.GetUserScoresAsync(user.Username, new GetScoresRequest(), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.Items.Select(i => i.Amount).Should().ContainInOrder(20, 10);
        result.Data.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetUserScoresAsync_should_return_only_user_score()
    {
        await using var db = TestDbContextFactory.Create();
        var user = new User {Email = "test@example.com", Username = "test"};
        db.Users.Add(user);
        var user2 = new User {Email = "test2@example.com", Username = "test2"};
        db.Users.Add(user2);
        db.Scores.AddRange(
            new Score {Amount = 10, CreatedAt = DateTime.UtcNow, User = user},
            new Score {Amount = 20, CreatedAt = DateTime.UtcNow, User = user},
            new Score {Amount = 30, CreatedAt = DateTime.UtcNow, User = user2}
        );
        await db.SaveChangesAsync();

        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var result = await service.GetUserScoresAsync(user.Username, new GetScoresRequest(), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.Items.Select(i => i.Amount).Should().ContainInOrder(20, 10);
        result.Data.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetUserScoresAsync_should_return_empty_list_when_user_has_no_score()
    {
        await using var db = TestDbContextFactory.Create();
        var user = new User {Email = "test@example.com", Username = "test"};
        var user2 = new User {Email = "test2@example.com", Username = "test2"};
        db.Scores.AddRange(
            new Score {Amount = 10, CreatedAt = DateTime.UtcNow, User = user},
            new Score {Amount = 20, CreatedAt = DateTime.UtcNow, User = user}
        );
        await db.SaveChangesAsync();

        var service = new LeaderboardService(db, NullLogger<LeaderboardService>.Instance);

        var result = await service.GetUserScoresAsync(user2.Username, new GetScoresRequest(), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.TotalCount.Should().Be(0);
    }
}
