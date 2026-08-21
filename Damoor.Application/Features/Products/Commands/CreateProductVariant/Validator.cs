using FluentValidation;

namespace Damoor.Application.Features.Products.Commands.CreateProductVariant;

public sealed class Validator : AbstractValidator<CreateProductVariantCommand>
{
    public Validator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);

        RuleFor(x => x.Variants)
            .NotEmpty()
            .WithMessage("At least one variant is required.");

        RuleForEach(x => x.Variants).SetValidator(new VariantItemValidator());
    }
}

public sealed class VariantItemValidator
    : AbstractValidator<CreateProductVariantItem>
{
    public VariantItemValidator()
    {
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

        RuleFor(x => x.SalePrice!.Value)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(x => x.Price)
            .When(x => x.SalePrice.HasValue)
            .WithName(nameof(CreateProductVariantItem.SalePrice));

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Images)
            .NotNull();

        RuleForEach(x => x.Images).SetValidator(new VariantImageItemValidator());

        RuleFor(x => x.Images)
            .Must(images => images.Count(i => i.IsMain) == 1)
            .When(x => x.Images is { Count: > 0 })
            .WithMessage("Each variant must have exactly one main image.");
    }
}

public sealed class VariantImageItemValidator
    : AbstractValidator<CreateProductVariantImageItem>
{
    public VariantImageItemValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .MaximumLength(2048);
    }
}
