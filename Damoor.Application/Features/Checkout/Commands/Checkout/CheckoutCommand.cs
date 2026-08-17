using Damoor.Application.Features.Orders.Models;
using MediatR;

namespace Damoor.Application.Features.Checkout.Commands.Checkout;

public sealed record CheckoutCommand(
    string? SessionToken,
    int? UserId,
    string ShippingAddress) : IRequest<OrderDetailsResult>;
