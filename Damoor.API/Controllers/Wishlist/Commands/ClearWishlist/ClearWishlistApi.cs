using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Wishlists.Commands.ClearWishlist;
using Damoor.Application.Features.Wishlists.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Wishlist;

public sealed partial class WishlistController
{
    [HttpDelete]
    [ProducesResponseType(
        typeof(ApiResponse<WishlistResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<WishlistResult>>> Clear(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ClearWishlistCommand(User.GetUserId()!.Value),
            cancellationToken);

        return OkResponse(result, "Wishlist cleared successfully.");
    }
}
