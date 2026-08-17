using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Carts.Commands.RemoveCartItem;
using Damoor.Application.Features.Carts.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Cart;

public sealed partial class CartController
{
    [HttpDelete("items/{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<CartResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CartResult>>> RemoveItem(
        int id,
        [FromHeader(Name = "X-Shopping-Session")] string? sessionToken,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RemoveCartItemCommand(sessionToken, User.GetUserId(), id),
            cancellationToken);

        return OkResponse(result, "Item removed from cart.");
    }
}
