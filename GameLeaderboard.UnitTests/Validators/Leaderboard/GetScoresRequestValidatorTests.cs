using FluentValidation.TestHelper;
using GameLeaderboard.Infrastructure.DTOs.Leaderboard;
using GameLeaderboard.Infrastructure.Validators.Leaderboard;

namespace GameLeaderboard.UnitTests.Validators.Leaderboard;

public class GetScoresRequestValidatorTests
{
    private readonly GetScoresRequestValidator validator = new();

    [Fact]
    public void Should_pass_with_valid_request()
    {
        var request = new GetScoresRequest();

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_fail_with_invalid_page(int page)
    {
        var request = new GetScoresRequest(page);

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Should_fail_with_invalid_page_size(int pageSize)
    {
        var request = new GetScoresRequest(1, pageSize);

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.PageSize);
    }
}
