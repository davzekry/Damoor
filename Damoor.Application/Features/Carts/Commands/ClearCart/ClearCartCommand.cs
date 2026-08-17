using Damoor.Application.Features.Carts.Models;
using MediatR;

namespace Damoor.Application.Features.Carts.Commands.ClearCart;

public sealed record ClearCartCommand(
    string? SessionToken,
    int? UserId) : IRequest<CartResult>;
