using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GameLeaderboard.Infrastructure.DTOs.Auth;

namespace GameLeaderboard.IntegrationTests;

public class AuthEndpointsTests : IntegrationTestBase
{
    public AuthEndpointsTests(GameLeaderboardWebAppFactory factory) : base(factory) {}

    [Fact]
    public async Task Register_then_login_should_return_token()
    {
        var register = await Client.PostAsJsonAsync("/api/Auth/register",
            new RegisterRequest("test", "test@example.com", "Password1"));
        register.StatusCode.Should().Be(HttpStatusCode.Created);

        var login = await Client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest("test@example.com", "Password1"));
        login.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await login.Content.ReadFromJsonAsync<LoginResponse>();
        body!.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_with_invalid_password_should_return_400()
    {
        var response = await Client.PostAsJsonAsync("api/Auth/register",
            new RegisterRequest("test", "test@example.com", "weak"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_with_unknown_email_should_return_401()
    {
        var response = await Client.PostAsJsonAsync("api/Auth/login",
            new LoginRequest("nobody@example.com", "Password1"));
        
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_with_wrong_password_should_return_401()
    {
        var register = await Client.PostAsJsonAsync("api/Auth/register",
            new RegisterRequest("test", "test@example.com", "Password1"));
        register.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await Client.PostAsJsonAsync("api/Auth/login",
            new LoginRequest("test@example.com", "WrongPassword1"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
