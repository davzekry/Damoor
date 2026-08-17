using FluentValidation;

namespace Damoor.Application.Features.Checkout.Commands.Checkout;

public sealed class Validator : AbstractValidator<CheckoutCommand>
{
    public Validator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty()
            .WithMessage("The X-Shopping-Session header is required.");

        RuleFor(x => x.ShippingAddress)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
