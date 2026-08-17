using Damoor.Application.Features.Wishlists.Models;
using MediatR;

namespace Damoor.Application.Features.Wishlists.Queries.GetWishlist;

public sealed record GetWishlistQuery(int UserId) : IRequest<WishlistResult>;
