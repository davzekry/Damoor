using FluentValidation;

namespace Damoor.Application.Features.Account.Commands.UpdateMe;

public sealed class Validator : AbstractValidator<UpdateMeCommand>
{
    public Validator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);
    }
}
