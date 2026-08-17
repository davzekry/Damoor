using FluentValidation;

namespace Damoor.Application.Features.Wishlists.Commands.ClearWishlist;

public sealed class Validator : AbstractValidator<ClearWishlistCommand>
{
    public Validator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}
