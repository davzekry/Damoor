using Damoor.Application.Features.Wishlists.Common;
using Damoor.Application.Features.Wishlists.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;

namespace Damoor.Application.Features.Wishlists.Queries.GetWishlist;

public sealed class GetWishlistHandler
    : IRequestHandler<GetWishlistQuery, WishlistResult>
{
    private readonly DamoorDbContext _db;

    public GetWishlistHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<WishlistResult> Handle(
        GetWishlistQuery request,
        CancellationToken cancellationToken)
    {
        var wishlistId = await WishlistAccessor.EnsureWishlistIdAsync(
            _db,
            request.UserId,
            cancellationToken);

        return await WishlistAccessor.BuildWishlistResultAsync(
            _db,
            wishlistId,
            cancellationToken);
    }
}
