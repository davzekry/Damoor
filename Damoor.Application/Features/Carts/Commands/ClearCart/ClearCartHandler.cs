using Damoor.Application.Features.Carts.Common;
using Damoor.Application.Features.Carts.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Carts.Commands.ClearCart;

public sealed class ClearCartHandler
    : IRequestHandler<ClearCartCommand, CartResult>
{
    private readonly DamoorDbContext _db;

    public ClearCartHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<CartResult> Handle(
        ClearCartCommand request,
        CancellationToken cancellationToken)
    {
        var cartId = await CartAccessor.ResolveCartIdAsync(
            _db,
            request.SessionToken,
            request.UserId,
            cancellationToken);

        var items = await _db.CartItems
            .Where(x => x.CartId == cartId)
            .ToListAsync(cancellationToken);

        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync(cancellationToken);

        return await CartAccessor.BuildCartResultAsync(_db, cartId, cancellationToken);
    }
}
