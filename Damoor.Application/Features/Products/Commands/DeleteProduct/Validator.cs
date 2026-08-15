using FluentValidation;

namespace Damoor.Application.Features.Products.Commands.DeleteProduct;

public sealed class Validator : AbstractValidator<DeleteProductCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
