using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Carts.Common;
using Damoor.Application.Features.Carts.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Carts.Commands.UpdateCartItem;

public sealed class UpdateCartItemHandler
    : IRequestHandler<UpdateCartItemCommand, CartResult>
{
    private readonly DamoorDbContext _db;

    public UpdateCartItemHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<CartResult> Handle(
        UpdateCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var cartId = await CartAccessor.ResolveCartIdAsync(
            _db,
            request.SessionToken,
            request.UserId,
            cancellationToken);

        var item = await _db.CartItems
            .Include(x => x.ProductVariant)
            .FirstOrDefaultAsync(
                x => x.ProductVariantId == request.ProductVariantId &&
                     x.CartId == cartId,
                cancellationToken);

        if (item is null)
            throw new NotFoundException("CartItem", request.ProductVariantId);

        if (item.ProductVariant.StockQuantity < request.Quantity)
            throw new BadRequestException("Insufficient stock for the requested quantity.");

        item.Quantity = request.Quantity;
        await _db.SaveChangesAsync(cancellationToken);

        return await CartAccessor.BuildCartResultAsync(_db, cartId, cancellationToken);
    }
}
