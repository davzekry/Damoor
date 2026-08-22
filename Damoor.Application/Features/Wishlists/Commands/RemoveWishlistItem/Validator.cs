using FluentValidation;

namespace Damoor.Application.Features.Wishlists.Commands.RemoveWishlistItem;

public sealed class Validator : AbstractValidator<RemoveWishlistItemCommand>
{
    public Validator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.ProductVariantId).GreaterThan(0);
    }
}
