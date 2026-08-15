using FluentValidation;

namespace Damoor.Application.Features.Products.Commands.DeleteProductVariant;

public sealed class Validator : AbstractValidator<DeleteProductVariantCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
