using FluentValidation;

namespace Damoor.Application.Features.Authentication.SignUp;

public sealed class SignUpValidator : AbstractValidator<SignUpCommand>
{
    private const string PhonePattern = @"^[+0-9\s\-()]{6,20}$";

    public SignUpValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(PhonePattern)
            .WithMessage("PhoneNumber must be a valid phone number.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(5)
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Password confirmation does not match.");
    }
}
