using Damoor.Application.Features.Wishlists.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Wishlists.Common;

internal static class WishlistAccessor
{
    public static async Task<int> EnsureWishlistIdAsync(
        DamoorDbContext db,
        int userId,
        CancellationToken cancellationToken)
    {
        var wishlistId = await db.Wishlists
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (wishlistId.HasValue)
            return wishlistId.Value;

        var wishlist = new Wishlist { UserId = userId };
        db.Wishlists.Add(wishlist);
        await db.SaveChangesAsync(cancellationToken);

        return wishlist.Id;
    }

    public static async Task<WishlistResult> BuildWishlistResultAsync(
        DamoorDbContext db,
        int wishlistId,
        CancellationToken cancellationToken)
    {
        var items = await db.WishlistItems
            .AsNoTracking()
            .Where(x => x.WishlistId == wishlistId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new WishlistItemResult
            {
                Id = x.Id,
                ProductVariantId = x.ProductVariantId,
                ProductId = x.ProductVariant.ProductId,
                ProductName = x.ProductVariant.Product.Name,
                SKU = x.ProductVariant.SKU,
                Size = x.ProductVariant.Size,
                Color = x.ProductVariant.Color,
                Price = x.ProductVariant.Price,
                IsInStock = x.ProductVariant.StockQuantity > 0,
                MainImageUrl = x.ProductVariant.Product.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.Id)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),
                AddedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new WishlistResult
        {
            WishlistId = wishlistId,
            Items = items
        };
    }
}
