using FluentAssertions;
using GameLeaderboard.Domain.Entities;
using GameLeaderboard.Infrastructure.DTOs.Auth;
using GameLeaderboard.Infrastructure.Services;
using GameLeaderboard.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace GameLeaderboard.UnitTests.Services;

public class AuthServiceTests
{
    private static IOptions<JwtSettings> JwtOptions() =>
        Options.Create(new JwtSettings
        {
            Secret = "test_long_enough_secret_for_hs256_!!!!!!!",
            ExpiryHours = 1
        });
    
    [Fact]
    public async Task RegisterAsync_should_fail_when_email_already_exist()
    {
        await using var db = TestDbContextFactory.Create();
        db.Users.Add(new User {Email = "test@example.com", Username = "test"});
        await db.SaveChangesAsync();

        var hasher = new Mock<IPasswordHasher<User>>();
        var service = new AuthService(db, JwtOptions(), hasher.Object, NullLogger<AuthService>.Instance);

        var request = new RegisterRequest("newTest", "test@example.com", "Password1");
        var result = await service.RegisterAsync(request, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Email already in use");
    }

    [Fact]
    public async Task RegisterAsync_should_persist_user_and_hash_password()
    {
        await using var db = TestDbContextFactory.Create();

        var hasher = new Mock<IPasswordHasher<User>>();
        hasher.Setup(h => h.HashPassword(It.IsAny<User>(), "Password1"))
            .Returns("HASHED");

        var service = new AuthService(db, JwtOptions(), hasher.Object, NullLogger<AuthService>.Instance);

        var request = new RegisterRequest("test", "test@example.com", "Password1");
        var result = await service.RegisterAsync(request, CancellationToken.None);
    
        result.Success.Should().BeTrue();
        db.Users.Should().ContainSingle(u =>
            u.Email == "test@example.com" && u.PasswordHash == "HASHED");
    }

    [Fact]
    public async Task LoginAsync_should_fail_when_user_not_found()
    {
        await using var db = TestDbContextFactory.Create();

        var hasher = new Mock<IPasswordHasher<User>>();
        var service = new AuthService(db, JwtOptions(), hasher.Object, NullLogger<AuthService>.Instance);

        var request = new LoginRequest("test@example.com", "Password1");
        var result = await service.LoginAsync(request, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_should_fail_when_password_is_invalid()
    {
        await using var db = TestDbContextFactory.Create();
        db.Users.Add(new User
        {
            Email = "test@example.com",
            Username = "test",
            PasswordHash = "HASHED"
        });
        await db.SaveChangesAsync();

        var hasher = new Mock<IPasswordHasher<User>>();
        hasher.Setup(h => h.VerifyHashedPassword(It.IsAny<User>(), "HASHED", "WrongPassword1"))
            .Returns(PasswordVerificationResult.Failed);

        var service = new AuthService(db, JwtOptions(), hasher.Object, NullLogger<AuthService>.Instance);

        var request = new LoginRequest("test@example.com", "WrongPassword1");
        var result = await service.LoginAsync(request, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_should_return_token_on_success()
    {
        await using var db = TestDbContextFactory.Create();
        db.Users.Add(new User
        {
            Email = "test@example.com",
            Username = "test",
            PasswordHash = "HASHED"
        });
        await db.SaveChangesAsync();

        var hasher = new Mock<IPasswordHasher<User>>();
        hasher.Setup(h => h.VerifyHashedPassword(It.IsAny<User>(), "HASHED", "Password1"))
            .Returns(PasswordVerificationResult.Success);

        var service = new AuthService(db, JwtOptions(), hasher.Object, NullLogger<AuthService>.Instance);

        var request = new LoginRequest("test@example.com", "Password1");
        var result = await service.LoginAsync(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrWhiteSpace();
    }
}
