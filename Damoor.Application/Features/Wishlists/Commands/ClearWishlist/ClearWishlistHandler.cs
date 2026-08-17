using Damoor.Application.Features.Wishlists.Common;
using Damoor.Application.Features.Wishlists.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Wishlists.Commands.ClearWishlist;

public sealed class ClearWishlistHandler
    : IRequestHandler<ClearWishlistCommand, WishlistResult>
{
    private readonly DamoorDbContext _db;

    public ClearWishlistHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<WishlistResult> Handle(
        ClearWishlistCommand request,
        CancellationToken cancellationToken)
    {
        var wishlistId = await WishlistAccessor.EnsureWishlistIdAsync(
            _db,
            request.UserId,
            cancellationToken);

        var items = await _db.WishlistItems
            .Where(x => x.WishlistId == wishlistId)
            .ToListAsync(cancellationToken);

        _db.WishlistItems.RemoveRange(items);
        await _db.SaveChangesAsync(cancellationToken);

        return await WishlistAccessor.BuildWishlistResultAsync(
            _db,
            wishlistId,
            cancellationToken);
    }
}
