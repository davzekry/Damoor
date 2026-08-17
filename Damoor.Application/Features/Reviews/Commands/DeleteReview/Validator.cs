using FluentValidation;

namespace Damoor.Application.Features.Reviews.Commands.DeleteReview;

public sealed class Validator : AbstractValidator<DeleteReviewCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}
