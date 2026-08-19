using FluentValidation;

namespace Damoor.Application.Features.Account.Queries.GetMe;

public sealed class Validator : AbstractValidator<GetMeQuery>
{
    public Validator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}
