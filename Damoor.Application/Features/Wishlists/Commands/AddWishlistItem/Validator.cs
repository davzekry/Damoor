using FluentValidation;

namespace Damoor.Application.Features.Wishlists.Commands.AddWishlistItem;

public sealed class Validator : AbstractValidator<AddWishlistItemCommand>
{
    public Validator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.ProductVariantId).GreaterThan(0);
    }
}
