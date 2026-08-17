using FluentValidation;

namespace Damoor.Application.Features.Carts.Commands.UpdateCartItem;

public sealed class Validator : AbstractValidator<UpdateCartItemCommand>
{
    public Validator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty()
            .WithMessage("The X-Shopping-Session header is required.");

        RuleFor(x => x.ItemId)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 100);
    }
}
