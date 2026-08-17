using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Carts.Models;
using Damoor.Application.Features.Carts.Queries.GetCart;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Cart;

public sealed partial class CartController
{
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<CartResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CartResult>>> Get(
        [FromHeader(Name = "X-Shopping-Session")] string? sessionToken,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCartQuery(sessionToken, User.GetUserId()),
            cancellationToken);

        return OkResponse(result);
    }
}
