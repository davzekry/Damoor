using FluentValidation;

namespace Damoor.Application.Features.Carts.Commands.ClearCart;

public sealed class Validator : AbstractValidator<ClearCartCommand>
{
    public Validator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty()
            .When(x => x.UserId is null)
            .WithMessage("The X-Shopping-Session header is required.");
    }
}
