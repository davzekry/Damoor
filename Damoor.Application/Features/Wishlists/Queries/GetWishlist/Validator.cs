using FluentValidation;

namespace Damoor.Application.Features.Wishlists.Queries.GetWishlist;

public sealed class Validator : AbstractValidator<GetWishlistQuery>
{
    public Validator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}
