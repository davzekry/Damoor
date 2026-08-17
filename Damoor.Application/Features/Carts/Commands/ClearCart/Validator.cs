using FluentValidation;

namespace Damoor.Application.Features.Carts.Commands.ClearCart;

public sealed class Validator : AbstractValidator<ClearCartCommand>
{
    public Validator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty()
            .WithMessage("The X-Shopping-Session header is required.");
    }
}
