using FluentValidation;
using GameLeaderboard.Infrastructure.DTOs.Leaderboard;

namespace GameLeaderboard.Infrastructure.Validators.Leaderboard;

public class SubmitScoreRequestValidator : AbstractValidator<SubmitScoreRequest>
{
    public SubmitScoreRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.CreatedAt)
            .NotEmpty().WithMessage("CreatedAt is required.");
    }
}
