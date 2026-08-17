using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Wishlists.Common;
using Damoor.Application.Features.Wishlists.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Wishlists.Commands.RemoveWishlistItem;

public sealed class RemoveWishlistItemHandler
    : IRequestHandler<RemoveWishlistItemCommand, WishlistResult>
{
    private readonly DamoorDbContext _db;

    public RemoveWishlistItemHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<WishlistResult> Handle(
        RemoveWishlistItemCommand request,
        CancellationToken cancellationToken)
    {
        var wishlistId = await WishlistAccessor.EnsureWishlistIdAsync(
            _db,
            request.UserId,
            cancellationToken);

        var item = await _db.WishlistItems
            .FirstOrDefaultAsync(
                x => x.WishlistId == wishlistId && x.ProductId == request.ProductId,
                cancellationToken);

        if (item is null)
            throw new NotFoundException("WishlistItem", request.ProductId);

        _db.WishlistItems.Remove(item);
        await _db.SaveChangesAsync(cancellationToken);

        return await WishlistAccessor.BuildWishlistResultAsync(
            _db,
            wishlistId,
            cancellationToken);
    }
}
