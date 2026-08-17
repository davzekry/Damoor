using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Orders.Common;
using Damoor.Application.Features.Orders.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdHandler
    : IRequestHandler<GetOrderByIdQuery, OrderDetailsResult>
{
    private readonly DamoorDbContext _db;

    public GetOrderByIdHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<OrderDetailsResult> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order is null)
            throw new NotFoundException("Order", request.Id);

        OrderAccessor.EnsureAccessible(order, request.UserId, request.SessionToken);

        return OrderAccessor.ToDetailsResult(order);
    }
}
