using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Wishlists.Commands.AddWishlistItem;
using Damoor.Application.Features.Wishlists.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Wishlist;

public sealed partial class WishlistController
{
    [HttpPost("items")]
    [ProducesResponseType(
        typeof(ApiResponse<WishlistResult>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<WishlistResult>>> AddItem(
        [FromBody] AddWishlistItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new AddWishlistItemCommand(User.GetUserId()!.Value, request.ProductId),
            cancellationToken);

        return CreatedResponse(result, "Item added to wishlist.");
    }
}

public sealed record AddWishlistItemRequest(int ProductId);
