using Damoor.Application.Features.Orders.Models;
using MediatR;

namespace Damoor.Application.Features.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(
    int Id,
    string? SessionToken,
    int? UserId) : IRequest<OrderDetailsResult>;
