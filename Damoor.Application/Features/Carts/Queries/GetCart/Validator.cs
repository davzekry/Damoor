using FluentValidation;

namespace Damoor.Application.Features.Carts.Queries.GetCart;

public sealed class Validator : AbstractValidator<GetCartQuery>
{
    public Validator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty()
            .When(x => x.UserId is null)
            .WithMessage("The X-Shopping-Session header is required.");
    }
}
