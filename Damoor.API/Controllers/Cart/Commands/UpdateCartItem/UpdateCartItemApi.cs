using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Carts.Commands.UpdateCartItem;
using Damoor.Application.Features.Carts.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Cart;

public sealed partial class CartController
{
    [HttpPut("items/{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<CartResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CartResult>>> UpdateItem(
        int id,
        [FromHeader(Name = "X-Shopping-Session")] string? sessionToken,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateCartItemCommand(
                sessionToken,
                User.GetUserId(),
                id,
                request.Quantity),
            cancellationToken);

        return OkResponse(result, "Cart item updated successfully.");
    }
}

public sealed record UpdateCartItemRequest(int Quantity);
