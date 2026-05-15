using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GameLeaderboard.Infrastructure.DTOs;
using GameLeaderboard.Infrastructure.DTOs.Auth;
using GameLeaderboard.Infrastructure.DTOs.Leaderboard;

namespace GameLeaderboard.IntegrationTests;

public class LeaderboardEndpointTests : IntegrationTestBase
{
    public LeaderboardEndpointTests(GameLeaderboardWebAppFactory factory) : base(factory) {}

    [Fact]
    public async Task Submit_without_token_should_return_401()
    {
        var response = await Client.PostAsJsonAsync("/api/Leaderboard/submit",
            new SubmitScoreRequest(100, DateTime.UtcNow));

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Submit_with_token_should_persist_and_return_201()
    {
        var token = await RegisterAndLoginAsync("test@example.com", "test", "Password1");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await Client.PostAsJsonAsync("api/Leaderboard/submit",
            new SubmitScoreRequest(100, DateTime.UtcNow));

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var score = await response.Content.ReadFromJsonAsync<ScoreDetails>();
        score!.Amount.Should().Be(100);
    }

    [Fact]
    public async Task GetAllScores_should_return_submitted_scores_sorted_desc()
    {
        var token = await RegisterAndLoginAsync("test@example.com", "test", "Password1");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await Client.PostAsJsonAsync("api/Leaderboard/submit",
            new SubmitScoreRequest(100, DateTime.UtcNow));
        await Client.PostAsJsonAsync("api/Leaderboard/submit",
            new SubmitScoreRequest(200, DateTime.UtcNow));
        await Client.PostAsJsonAsync("api/Leaderboard/submit",
            new SubmitScoreRequest(300, DateTime.UtcNow));

        Client.DefaultRequestHeaders.Authorization = null;

        var paged = await Client.GetFromJsonAsync<PagedResult<ScoreSummary>>("api/Leaderboard/scores");

        paged!.TotalCount.Should().Be(3);
        paged.Items.Select(i => i.Amount).Should().ContainInOrder(300, 200, 100);
    }

    private async Task<string> RegisterAndLoginAsync(string email, string username, string password)
    {
        var register = await Client.PostAsJsonAsync("api/Auth/register",
            new RegisterRequest(username, email, password));
        register.EnsureSuccessStatusCode();
        
        var login = await Client.PostAsJsonAsync("api/Auth/login",
            new LoginRequest(email, password));
        login.EnsureSuccessStatusCode();

        var body = await login.Content.ReadFromJsonAsync<LoginResponse>();
        return body!.Token;
    }
}
