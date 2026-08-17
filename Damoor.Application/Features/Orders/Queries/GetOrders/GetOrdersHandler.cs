using Damoor.Application.Features.Orders.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Orders.Queries.GetOrders;

public sealed class GetOrdersHandler
    : IRequestHandler<GetOrdersQuery, List<OrderSummaryResult>>
{
    private readonly DamoorDbContext _db;

    public GetOrdersHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<List<OrderSummaryResult>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.Orders.AsNoTracking().AsQueryable();

        query = request.UserId.HasValue
            ? query.Where(x => x.UserId == request.UserId.Value)
            : query.Where(x => x.SessionToken == request.SessionToken);

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new OrderSummaryResult
            {
                Id = x.Id,
                Status = x.Status,
                TotalAmount = x.TotalAmount,
                CreatedAt = x.CreatedAt,
                ItemCount = x.Items.Count
            })
            .ToListAsync(cancellationToken);
    }
}
