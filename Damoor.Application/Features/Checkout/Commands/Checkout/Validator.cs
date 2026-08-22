using FluentValidation;

namespace Damoor.Application.Features.Checkout.Commands.Checkout;

public sealed class Validator : AbstractValidator<CheckoutCommand>
{
    private const string PhonePattern = @"^[+0-9\s\-()]{6,32}$";

    public Validator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty()
            .WithMessage("The X-Shopping-Session header is required.");

        RuleFor(x => x.ShippingAddress)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.WhatsAppNumber)
            .NotEmpty()
            .MaximumLength(32)
            .Matches(PhonePattern)
            .WithMessage("WhatsAppNumber must be a valid phone number.");

        RuleFor(x => x.BackupPhoneNumber)
            .MaximumLength(32)
            .Matches(PhonePattern)
            .WithMessage("BackupPhoneNumber must be a valid phone number.")
            .When(x => !string.IsNullOrWhiteSpace(x.BackupPhoneNumber));

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => x.Notes is not null);
    }
}
