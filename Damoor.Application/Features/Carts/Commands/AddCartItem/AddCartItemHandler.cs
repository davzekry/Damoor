using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Carts.Common;
using Damoor.Application.Features.Carts.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Carts.Commands.AddCartItem;

public sealed class AddCartItemHandler
    : IRequestHandler<AddCartItemCommand, CartResult>
{
    private readonly DamoorDbContext _db;

    public AddCartItemHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<CartResult> Handle(
        AddCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var cartId = await CartAccessor.ResolveCartIdAsync(
            _db,
            request.SessionToken,
            request.UserId,
            cancellationToken);

        var variant = await _db.ProductVariants
            .FirstOrDefaultAsync(
                x => x.Id == request.ProductVariantId,
                cancellationToken);

        if (variant is null)
            throw new NotFoundException("ProductVariant", request.ProductVariantId);

        var existingItem = await _db.CartItems
            .FirstOrDefaultAsync(
                x => x.CartId == cartId && x.ProductVariantId == request.ProductVariantId,
                cancellationToken);

        var desiredQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;

        if (variant.StockQuantity < desiredQuantity)
            throw new BadRequestException("Insufficient stock for the requested quantity.");

        if (existingItem is null)
        {
            _db.CartItems.Add(new CartItem
            {
                CartId = cartId,
                ProductVariantId = request.ProductVariantId,
                Quantity = request.Quantity
            });
        }
        else
        {
            existingItem.Quantity = desiredQuantity;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return await CartAccessor.BuildCartResultAsync(_db, cartId, cancellationToken);
    }
}
