using Damoor.Application.Features.Carts.Models;
using MediatR;

namespace Damoor.Application.Features.Carts.Queries.GetCart;

public sealed record GetCartQuery(
    string? SessionToken,
    int? UserId) : IRequest<CartResult>;
