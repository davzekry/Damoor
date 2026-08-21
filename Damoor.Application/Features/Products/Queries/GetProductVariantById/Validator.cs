using FluentValidation;

namespace Damoor.Application.Features.Products.Queries.GetProductVariantById;

public sealed class Validator : AbstractValidator<GetProductVariantByIdQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
