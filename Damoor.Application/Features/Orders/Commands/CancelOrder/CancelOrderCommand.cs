using Damoor.Application.Features.Orders.Models;
using MediatR;

namespace Damoor.Application.Features.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(
    int Id,
    string? SessionToken,
    int? UserId) : IRequest<OrderDetailsResult>;
