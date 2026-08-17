using FluentValidation;

namespace Damoor.Application.Features.Reviews.Commands.CreateReview;

public sealed class Validator : AbstractValidator<CreateReviewCommand>
{
    public Validator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.UserId).GreaterThan(0);

        RuleFor(x => x.Rating).InclusiveBetween(1, 5);

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .When(x => x.Comment is not null);
    }
}
