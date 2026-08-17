using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Carts.Commands.AddCartItem;
using Damoor.Application.Features.Carts.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Cart;

public sealed partial class CartController
{
    [HttpPost("items")]
    [ProducesResponseType(
        typeof(ApiResponse<CartResult>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CartResult>>> AddItem(
        [FromHeader(Name = "X-Shopping-Session")] string? sessionToken,
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new AddCartItemCommand(
                sessionToken,
                User.GetUserId(),
                request.ProductVariantId,
                request.Quantity),
            cancellationToken);

        return CreatedResponse(result, "Item added to cart.");
    }
}

public sealed record AddCartItemRequest(int ProductVariantId, int Quantity);
