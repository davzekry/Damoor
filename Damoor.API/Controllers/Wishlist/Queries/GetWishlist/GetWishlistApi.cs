using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Wishlists.Models;
using Damoor.Application.Features.Wishlists.Queries.GetWishlist;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Wishlist;

public sealed partial class WishlistController
{
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<WishlistResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<WishlistResult>>> Get(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetWishlistQuery(User.GetUserId()!.Value),
            cancellationToken);

        return OkResponse(result);
    }
}
