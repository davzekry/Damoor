using Damoor.Application.Features.Carts.Models;
using MediatR;

namespace Damoor.Application.Features.Carts.Commands.RemoveCartItem;

public sealed record RemoveCartItemCommand(
    string? SessionToken,
    int? UserId,
    int ProductVariantId) : IRequest<CartResult>;
