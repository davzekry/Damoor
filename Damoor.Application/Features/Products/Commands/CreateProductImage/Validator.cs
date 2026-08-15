using FluentValidation;

namespace Damoor.Application.Features.Products.Commands.CreateProductImage;

public sealed class Validator : AbstractValidator<CreateProductImageCommand>
{
    public Validator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);

        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .MaximumLength(2048);
    }
}
