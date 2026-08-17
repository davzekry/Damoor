using Damoor.Application.Features.Carts.Common;
using Damoor.Application.Features.Carts.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;

namespace Damoor.Application.Features.Carts.Queries.GetCart;

public sealed class GetCartHandler
    : IRequestHandler<GetCartQuery, CartResult>
{
    private readonly DamoorDbContext _db;

    public GetCartHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<CartResult> Handle(
        GetCartQuery request,
        CancellationToken cancellationToken)
    {
        var cartId = await CartAccessor.ResolveCartIdAsync(
            _db,
            request.SessionToken,
            request.UserId,
            cancellationToken);

        return await CartAccessor.BuildCartResultAsync(_db, cartId, cancellationToken);
    }
}
