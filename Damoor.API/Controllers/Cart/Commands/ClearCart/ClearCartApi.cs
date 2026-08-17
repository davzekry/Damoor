using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Carts.Commands.ClearCart;
using Damoor.Application.Features.Carts.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Cart;

public sealed partial class CartController
{
    [HttpDelete]
    [ProducesResponseType(
        typeof(ApiResponse<CartResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CartResult>>> Clear(
        [FromHeader(Name = "X-Shopping-Session")] string? sessionToken,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ClearCartCommand(sessionToken, User.GetUserId()),
            cancellationToken);

        return OkResponse(result, "Cart cleared successfully.");
    }
}
