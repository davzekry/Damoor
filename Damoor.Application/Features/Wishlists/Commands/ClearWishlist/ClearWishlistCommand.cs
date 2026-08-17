using Damoor.Application.Features.Wishlists.Models;
using MediatR;

namespace Damoor.Application.Features.Wishlists.Commands.ClearWishlist;

public sealed record ClearWishlistCommand(int UserId) : IRequest<WishlistResult>;
