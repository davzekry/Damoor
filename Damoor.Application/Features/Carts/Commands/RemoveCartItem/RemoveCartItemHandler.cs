using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Carts.Common;
using Damoor.Application.Features.Carts.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Carts.Commands.RemoveCartItem;

public sealed class RemoveCartItemHandler
    : IRequestHandler<RemoveCartItemCommand, CartResult>
{
    private readonly DamoorDbContext _db;

    public RemoveCartItemHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<CartResult> Handle(
        RemoveCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var cartId = await CartAccessor.ResolveCartIdAsync(
            _db,
            request.SessionToken,
            request.UserId,
            cancellationToken);

        var item = await _db.CartItems
            .FirstOrDefaultAsync(
                x => x.ProductVariantId == request.ProductVariantId &&
                     x.CartId == cartId,
                cancellationToken);

        if (item is null)
            throw new NotFoundException("CartItem", request.ProductVariantId);

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync(cancellationToken);

        return await CartAccessor.BuildCartResultAsync(_db, cartId, cancellationToken);
    }
}
