using Damoor.Application.Features.Wishlists.Models;
using MediatR;

namespace Damoor.Application.Features.Wishlists.Commands.RemoveWishlistItem;

public sealed record RemoveWishlistItemCommand(
    int UserId,
    int ProductVariantId) : IRequest<WishlistResult>;
