using FluentValidation;

namespace Damoor.Application.Features.Products.Queries.GetProductById;

public sealed class Validator : AbstractValidator<GetProductByIdQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
