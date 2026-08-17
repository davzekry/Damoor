using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Orders.Common;
using Damoor.Application.Features.Orders.Models;
using Damoor.Domain.Entities.Enums;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Orders.Commands.UpdateOrderStatus;

public sealed class UpdateOrderStatusHandler
    : IRequestHandler<UpdateOrderStatusCommand, AdminOrderDetailsResult>
{
    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
    {
        [OrderStatus.Pending] = [OrderStatus.Confirmed, OrderStatus.Cancelled],
        [OrderStatus.Confirmed] = [OrderStatus.Processing, OrderStatus.Cancelled],
        [OrderStatus.Processing] = [OrderStatus.Shipped, OrderStatus.Cancelled],
        [OrderStatus.Shipped] = [OrderStatus.Delivered],
        [OrderStatus.Delivered] = [],
        [OrderStatus.Cancelled] = []
    };

    private readonly DamoorDbContext _db;

    public UpdateOrderStatusHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<AdminOrderDetailsResult> Handle(
        UpdateOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _db.Orders
            .Include(x => x.Items)
                .ThenInclude(x => x.ProductVariant)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order is null)
            throw new NotFoundException("Order", request.Id);

        if (order.Status == request.Status)
            throw new ConflictException("Order is already in the requested status.");

        if (!AllowedTransitions[order.Status].Contains(request.Status))
        {
            throw new ConflictException(
                $"Cannot transition an order from '{order.Status}' to '{request.Status}'.");
        }

        if (request.Status == OrderStatus.Cancelled)
            OrderAccessor.RestockItems(order);

        order.Status = request.Status;
        await _db.SaveChangesAsync(cancellationToken);

        var user = order.UserId.HasValue
            ? await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == order.UserId.Value, cancellationToken)
            : null;

        return OrderAccessor.ToAdminDetailsResult(order, user);
    }
}
