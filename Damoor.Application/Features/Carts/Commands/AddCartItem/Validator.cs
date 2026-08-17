using FluentValidation;

namespace Damoor.Application.Features.Carts.Commands.AddCartItem;

public sealed class Validator : AbstractValidator<AddCartItemCommand>
{
    public Validator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty()
            .WithMessage("The X-Shopping-Session header is required.");

        RuleFor(x => x.ProductVariantId)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 100);
    }
}
