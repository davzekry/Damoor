using FluentValidation;

namespace Damoor.Application.Features.Carts.Commands.RemoveCartItem;

public sealed class Validator : AbstractValidator<RemoveCartItemCommand>
{
    public Validator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty()
            .When(x => x.UserId is null)
            .WithMessage("The X-Shopping-Session header is required.");

        RuleFor(x => x.ItemId)
            .GreaterThan(0);
    }
}
