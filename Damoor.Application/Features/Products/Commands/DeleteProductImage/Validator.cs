using FluentValidation;

namespace Damoor.Application.Features.Products.Commands.DeleteProductImage;

public sealed class Validator : AbstractValidator<DeleteProductImageCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
