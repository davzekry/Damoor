using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Carts.Models;
using Damoor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Carts.Common;

internal static class CartAccessor
{
    public static async Task<int> ResolveCartIdAsync(
        DamoorDbContext db,
        string? sessionToken,
        int? userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
            throw new BadRequestException("The X-Shopping-Session header is required.");

        var session = await db.ShoppingSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.SessionToken == sessionToken,
                cancellationToken);

        if (session is null || session.ExpiresAt <= DateTime.UtcNow)
            throw new NotFoundException("ShoppingSession", sessionToken);

        if (session.UserId.HasValue && session.UserId != userId)
            throw new UnauthorizedException(
                "This shopping session does not belong to the current user.");

        var cart = await db.Carts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ShoppingSessionId == session.Id,
                cancellationToken);

        if (cart is null)
            throw new NotFoundException("Cart", sessionToken);

        return cart.Id;
    }

    public static async Task<CartResult> BuildCartResultAsync(
        DamoorDbContext db,
        int cartId,
        CancellationToken cancellationToken)
    {
        var items = await db.CartItems
            .AsNoTracking()
            .Where(x => x.CartId == cartId)
            .OrderBy(x => x.Id)
            .Select(x => new CartItemResult
            {
                Id = x.Id,
                ProductId = x.ProductVariant.ProductId,
                ProductName = x.ProductVariant.Product.Name,
                ProductVariantId = x.ProductVariantId,
                SKU = x.ProductVariant.SKU,
                Size = x.ProductVariant.Size,
                Color = x.ProductVariant.Color,
                UnitPrice = x.ProductVariant.Price,
                Quantity = x.Quantity,
                MainImageUrl = x.ProductVariant.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.Id)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault()
                    ?? x.ProductVariant.Product.Images
                        .Where(i => i.ProductVariantId == null)
                        .OrderByDescending(i => i.IsMain)
                        .ThenBy(i => i.Id)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return new CartResult
        {
            CartId = cartId,
            Items = items
        };
    }
}
