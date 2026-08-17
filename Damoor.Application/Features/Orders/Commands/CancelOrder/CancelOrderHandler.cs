using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Orders.Common;
using Damoor.Application.Features.Orders.Models;
using Damoor.Domain.Entities.Enums;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Orders.Commands.CancelOrder;

public sealed class CancelOrderHandler
    : IRequestHandler<CancelOrderCommand, OrderDetailsResult>
{
    private readonly DamoorDbContext _db;

    public CancelOrderHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<OrderDetailsResult> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _db.Orders
            .Include(x => x.Items)
                .ThenInclude(x => x.ProductVariant)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order is null)
            throw new NotFoundException("Order", request.Id);

        OrderAccessor.EnsureAccessible(order, request.UserId, request.SessionToken);

        if (order.Status is not (OrderStatus.Pending or OrderStatus.Confirmed))
            throw new ConflictException("Only pending or confirmed orders can be cancelled.");

        foreach (var item in order.Items.Where(x => x.ProductVariant is not null))
            item.ProductVariant!.StockQuantity += item.Quantity;

        order.Status = OrderStatus.Cancelled;
        await _db.SaveChangesAsync(cancellationToken);

        return OrderAccessor.ToDetailsResult(order);
    }
}
