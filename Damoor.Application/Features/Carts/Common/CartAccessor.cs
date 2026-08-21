using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Carts.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Carts.Common;

internal static class CartAccessor
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromDays(30);

    public static async Task<int> ResolveCartIdAsync(
        DamoorDbContext db,
        string? sessionToken,
        int? userId,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // Signed-in shopper: use (or create) the cart tied to their account,
        // independent of any X-Shopping-Session header.
        if (userId.HasValue)
            return await ResolveUserCartIdAsync(db, userId.Value, now, cancellationToken);

        // Guest shopper: identified solely by the shopping-session token.
        if (string.IsNullOrWhiteSpace(sessionToken))
            throw new BadRequestException("The X-Shopping-Session header is required.");

        var session = await db.ShoppingSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.SessionToken == sessionToken,
                cancellationToken);

        if (session is null || session.ExpiresAt <= now)
            throw new NotFoundException("ShoppingSession", sessionToken);

        if (session.UserId.HasValue)
            throw new UnauthorizedException(
                "This shopping session belongs to a registered account. Please sign in.");

        var cart = await db.Carts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ShoppingSessionId == session.Id,
                cancellationToken);

        if (cart is null)
            throw new NotFoundException("Cart", sessionToken);

        return cart.Id;
    }

    private static async Task<int> ResolveUserCartIdAsync(
        DamoorDbContext db,
        int userId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var cartId = await db.Carts
            .Where(x =>
                x.ShoppingSession.UserId == userId &&
                x.ShoppingSession.ExpiresAt > now)
            .OrderByDescending(x => x.ShoppingSession.ExpiresAt)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (cartId.HasValue)
            return cartId.Value;

        // First interaction for this account: provision a session-backed cart.
        var session = new ShoppingSession
        {
            SessionToken = Guid.NewGuid().ToString("N"),
            UserId = userId,
            ExpiresAt = now.Add(SessionLifetime),
            Cart = new Cart()
        };

        db.ShoppingSessions.Add(session);
        await db.SaveChangesAsync(cancellationToken);

        return session.Cart.Id;
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
