using Damoor.Application.Features.Carts.Models;
using MediatR;

namespace Damoor.Application.Features.Carts.Commands.UpdateCartItem;

public sealed record UpdateCartItemCommand(
    string? SessionToken,
    int? UserId,
    int ProductVariantId,
    int Quantity) : IRequest<CartResult>;
