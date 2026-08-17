using Damoor.Application.Features.Orders.Models;
using MediatR;

namespace Damoor.Application.Features.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery(
    string? SessionToken,
    int? UserId) : IRequest<List<OrderSummaryResult>>;
