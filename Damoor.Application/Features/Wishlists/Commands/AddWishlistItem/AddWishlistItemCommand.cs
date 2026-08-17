using Damoor.Application.Features.Wishlists.Models;
using MediatR;

namespace Damoor.Application.Features.Wishlists.Commands.AddWishlistItem;

public sealed record AddWishlistItemCommand(
    int UserId,
    int ProductId) : IRequest<WishlistResult>;
