using FluentValidation;

namespace Damoor.Application.Features.Reviews.Queries.GetVariantReviews;

public sealed class Validator : AbstractValidator<GetVariantReviewsQuery>
{
    public Validator()
    {
        RuleFor(x => x.ProductVariantId).GreaterThan(0);
    }
}
