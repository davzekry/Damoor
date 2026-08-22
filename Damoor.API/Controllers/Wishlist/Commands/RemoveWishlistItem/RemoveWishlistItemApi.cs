using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Wishlists.Commands.RemoveWishlistItem;
using Damoor.Application.Features.Wishlists.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Wishlist;

public sealed partial class WishlistController
{
    [HttpDelete("items/{productVariantId:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<WishlistResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<WishlistResult>>> RemoveItem(
        int productVariantId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RemoveWishlistItemCommand(User.GetUserId()!.Value, productVariantId),
            cancellationToken);

        return OkResponse(result, "Item removed from wishlist.");
    }
}
