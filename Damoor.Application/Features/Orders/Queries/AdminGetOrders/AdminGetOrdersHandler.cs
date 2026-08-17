using Damoor.Application.Common.Models;
using Damoor.Application.Features.Orders.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Orders.Queries.AdminGetOrders;

public sealed class AdminGetOrdersHandler
    : IRequestHandler<AdminGetOrdersQuery, PaginatedList<AdminOrderSummaryResult>>
{
    private readonly DamoorDbContext _db;

    public AdminGetOrdersHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedList<AdminOrderSummaryResult>> Handle(
        AdminGetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var search = request.Search?.Trim();

        var query =
            from o in _db.Orders.AsNoTracking()
            join u in _db.Users.AsNoTracking() on o.UserId equals u.Id into userJoin
            from u in userJoin.DefaultIfEmpty()
            select new { Order = o, User = u };

        if (request.Status.HasValue)
            query = query.Where(x => x.Order.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                (x.Order.SessionToken != null && x.Order.SessionToken.Contains(search)) ||
                (x.User != null &&
                    (x.User.FullName.Contains(search) ||
                     (x.User.Email != null && x.User.Email.Contains(search)))));
        }

        query = request.Asc
            ? query.OrderBy(x => x.Order.CreatedAt).ThenBy(x => x.Order.Id)
            : query.OrderByDescending(x => x.Order.CreatedAt).ThenBy(x => x.Order.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AdminOrderSummaryResult
            {
                Id = x.Order.Id,
                Status = x.Order.Status,
                TotalAmount = x.Order.TotalAmount,
                CreatedAt = x.Order.CreatedAt,
                ItemCount = x.Order.Items.Count,
                UserId = x.Order.UserId,
                CustomerName = x.User != null ? x.User.FullName : null,
                CustomerEmail = x.User != null ? x.User.Email : null,
                SessionToken = x.Order.SessionToken
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<AdminOrderSummaryResult>(
            items,
            totalCount,
            request.Page,
            request.PageSize);
    }
}
