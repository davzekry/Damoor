using FluentValidation;

namespace Damoor.Application.Features.Carts.Queries.GetCart;

public sealed class Validator : AbstractValidator<GetCartQuery>
{
    public Validator()
    {
        RuleFor(x => x.SessionToken)
            .NotEmpty()
            .WithMessage("The X-Shopping-Session header is required.");
    }
}
