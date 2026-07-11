using FluentValidation;

namespace Damoor.Application.Features.Products.Queries.GetProductVariants;

public sealed class Validator : AbstractValidator<GetProductVariantsQuery>
{
    public Validator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
    }
}
