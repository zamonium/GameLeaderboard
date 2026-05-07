using FluentValidation;
using GameLeaderboard.Infrastructure.DTOs.Leaderboard;

namespace GameLeaderboard.Infrastructure.Validators.Leaderboard;

public class GetScoresRequestValidator : AbstractValidator<GetScoresRequest>
{
    public GetScoresRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize must be at least 1.")
            .LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100.");
    }
}
