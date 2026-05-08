using FluentValidation.TestHelper;
using GameLeaderboard.Infrastructure.DTOs.Leaderboard;
using GameLeaderboard.Infrastructure.Validators.Leaderboard;

namespace GameLeaderboard.UnitTests.Validators.Leaderboard;

public class SubmitScoreRequestValidatorTests
{
    private readonly SubmitScoreRequestValidator validator = new();

    [Fact]
    public void Should_pass_with_valid_request()
    {
        var request = new SubmitScoreRequest(10, DateTime.UtcNow);

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_fail_with_invalid_amount(int amount)
    {
        var request = new SubmitScoreRequest(amount, DateTime.UtcNow);

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Amount);
    }

    [Fact]
    public void Should_fail_with_invalid_created_at()
    {
        var request = new SubmitScoreRequest(10, default);

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.CreatedAt);
    }
}
