using Damoor.Application.Features.Carts.Models;
using MediatR;

namespace Damoor.Application.Features.Carts.Commands.AddCartItem;

public sealed record AddCartItemCommand(
    string? SessionToken,
    int? UserId,
    int ProductVariantId,
    int Quantity) : IRequest<CartResult>;
