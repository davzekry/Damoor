using Damoor.Application.Common.Models;
using Damoor.Application.Features.Orders.Models;
using Damoor.Domain.Entities.Enums;
using MediatR;

namespace Damoor.Application.Features.Orders.Queries.AdminGetOrders;

public sealed record AdminGetOrdersQuery(
    int Page = 1,
    int PageSize = 10,
    OrderStatus? Status = null,
    string? Search = null,
    bool Asc = false) : IRequest<PaginatedList<AdminOrderSummaryResult>>;
