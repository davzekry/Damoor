using FluentValidation;

namespace Damoor.Application.Features.Authentication.ChangePassword;

public sealed class Validator : AbstractValidator<ChangePasswordCommand>
{
    public Validator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);

        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(5)
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.");
    }
}
