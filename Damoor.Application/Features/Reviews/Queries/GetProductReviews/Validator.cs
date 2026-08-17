using FluentValidation;

namespace Damoor.Application.Features.Reviews.Queries.GetProductReviews;

public sealed class Validator : AbstractValidator<GetProductReviewsQuery>
{
    public Validator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
    }
}
