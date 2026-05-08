using FluentValidation.TestHelper;
using GameLeaderboard.Infrastructure.DTOs.Auth;
using GameLeaderboard.Infrastructure.Validators.Auth;

namespace GameLeaderboard.UnitTests.Validators.Auth;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator validator = new();

    [Fact]
    public void Should_pass_with_valid_request()
    {
        var request = new LoginRequest("test@example.com", "Password1");

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("test")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [InlineData("a_very_long_email_that_exceeds_the_limit_of_fifty_chars@example.com")]
    public void Should_fail_with_invalid_email(string email)
    {
        var request = new LoginRequest(email, "Password1");

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A_very_long_password_that_exceeds_the_limit_of_fifty_chars1")]
    public void Should_fail_with_invalid_password(string password)
    {
        var request = new LoginRequest("test@example.com", password);

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Password);
    }
}
