using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Orders.Common;
using Damoor.Application.Features.Orders.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Orders.Queries.AdminGetOrderById;

public sealed class AdminGetOrderByIdHandler
    : IRequestHandler<AdminGetOrderByIdQuery, AdminOrderDetailsResult>
{
    private readonly DamoorDbContext _db;

    public AdminGetOrderByIdHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<AdminOrderDetailsResult> Handle(
        AdminGetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order is null)
            throw new NotFoundException("Order", request.Id);

        var user = order.UserId.HasValue
            ? await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == order.UserId.Value, cancellationToken)
            : null;

        return OrderAccessor.ToAdminDetailsResult(order, user);
    }
}
