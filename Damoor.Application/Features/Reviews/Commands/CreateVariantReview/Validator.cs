using FluentValidation;

namespace Damoor.Application.Features.Reviews.Commands.CreateVariantReview;

public sealed class Validator : AbstractValidator<CreateVariantReviewCommand>
{
    public Validator()
    {
        RuleFor(x => x.ProductVariantId).GreaterThan(0);
        RuleFor(x => x.UserId).GreaterThan(0);

        RuleFor(x => x.Rating).InclusiveBetween(1, 5);

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .When(x => x.Comment is not null);
    }
}
