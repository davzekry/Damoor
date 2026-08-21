using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Wishlists.Common;
using Damoor.Application.Features.Wishlists.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Wishlists.Commands.AddWishlistItem;

public sealed class AddWishlistItemHandler
    : IRequestHandler<AddWishlistItemCommand, WishlistResult>
{
    private readonly DamoorDbContext _db;

    public AddWishlistItemHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<WishlistResult> Handle(
        AddWishlistItemCommand request,
        CancellationToken cancellationToken)
    {
        var wishlistId = await WishlistAccessor.EnsureWishlistIdAsync(
            _db,
            request.UserId,
            cancellationToken);

        var variantExists = await _db.ProductVariants
            .AnyAsync(x => x.Id == request.ProductVariantId, cancellationToken);

        if (!variantExists)
            throw new NotFoundException("ProductVariant", request.ProductVariantId);

        var alreadyInWishlist = await _db.WishlistItems
            .AnyAsync(
                x => x.WishlistId == wishlistId &&
                     x.ProductVariantId == request.ProductVariantId,
                cancellationToken);

        // Adding an already-wishlisted variant is idempotent rather than a
        // conflict, since re-favoriting an item is not an error from the
        // customer's perspective.
        if (!alreadyInWishlist)
        {
            _db.WishlistItems.Add(new WishlistItem
            {
                WishlistId = wishlistId,
                ProductVariantId = request.ProductVariantId
            });

            await _db.SaveChangesAsync(cancellationToken);
        }

        return await WishlistAccessor.BuildWishlistResultAsync(
            _db,
            wishlistId,
            cancellationToken);
    }
}
