using FluentValidation;

namespace Damoor.Application.Features.Products.Commands.CreateProductVariant;

public sealed class Validator : AbstractValidator<CreateProductVariantCommand>
{
    public Validator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);

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
