using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Carts.Common;
using Damoor.Application.Features.Orders.Common;
using Damoor.Application.Features.Orders.Models;
using Damoor.Domain.Entities;
using Damoor.Domain.Entities.Enums;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Checkout.Commands.Checkout;

public sealed class CheckoutHandler
    : IRequestHandler<CheckoutCommand, OrderDetailsResult>
{
    private readonly DamoorDbContext _db;

    public CheckoutHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<OrderDetailsResult> Handle(
        CheckoutCommand request,
        CancellationToken cancellationToken)
    {
        var cartId = await CartAccessor.ResolveCartIdAsync(
            _db,
            request.SessionToken,
            request.UserId,
            cancellationToken);

        var cartItems = await _db.CartItems
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
            .Where(x => x.CartId == cartId)
            .ToListAsync(cancellationToken);

        if (cartItems.Count == 0)
            throw new BadRequestException("The cart is empty.");

        foreach (var cartItem in cartItems)
        {
            if (cartItem.ProductVariant.StockQuantity < cartItem.Quantity)
            {
                throw new BadRequestException(
                    $"Insufficient stock for variant '{cartItem.ProductVariant.SKU}'.");
            }
        }

        var orderItems = cartItems
            .Select(ci => new OrderItem
            {
                ProductVariantId = ci.ProductVariantId,
                ProductName = ci.ProductVariant.Product.Name,
                VariantDescription = $"{ci.ProductVariant.Size} / {ci.ProductVariant.Color}",
                Quantity = ci.Quantity,
                UnitPrice = ci.ProductVariant.Price
            })
            .ToList();

        var order = new Order
        {
            UserId = request.UserId,
            SessionToken = request.UserId.HasValue ? null : request.SessionToken,
            TotalAmount = orderItems.Sum(x => x.Quantity * x.UnitPrice),
            Status = OrderStatus.Pending,
            ShippingAddress = request.ShippingAddress.Trim(),
            CustomerName = request.CustomerName.Trim(),
            WhatsAppNumber = request.WhatsAppNumber.Trim(),
            BackupPhoneNumber = string.IsNullOrWhiteSpace(request.BackupPhoneNumber)
                ? null
                : request.BackupPhoneNumber.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes)
                ? null
                : request.Notes.Trim(),
            Items = orderItems
        };

        _db.Orders.Add(order);

        foreach (var cartItem in cartItems)
            cartItem.ProductVariant.StockQuantity -= cartItem.Quantity;

        _db.CartItems.RemoveRange(cartItems);

        // A single SaveChangesAsync call is already atomic; an explicit
        // BeginTransactionAsync would require an execution strategy because
        // DatabaseExtensions enables SQL Server connection retry.
        await _db.SaveChangesAsync(cancellationToken);

        return OrderAccessor.ToDetailsResult(order);
    }
}
