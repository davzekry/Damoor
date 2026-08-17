using Damoor.Application.Features.Orders.Models;
using Damoor.Domain.Entities.Enums;
using MediatR;

namespace Damoor.Application.Features.Orders.Commands.UpdateOrderStatus;

public sealed record UpdateOrderStatusCommand(
    int Id,
    OrderStatus Status) : IRequest<AdminOrderDetailsResult>;
