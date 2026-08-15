using FluentValidation;

namespace Damoor.Application.Features.Products.Commands.UpdateProductVariant;

public sealed class Validator : AbstractValidator<UpdateProductVariantCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.SKU)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.Size)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.Color)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0);
    }
}
