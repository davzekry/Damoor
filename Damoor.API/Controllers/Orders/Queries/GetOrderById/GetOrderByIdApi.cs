using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Orders.Models;
using Damoor.Application.Features.Orders.Queries.GetOrderById;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Orders;

public sealed partial class OrdersController
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<OrderDetailsResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<OrderDetailsResult>>> GetById(
        int id,
        [FromHeader(Name = "X-Shopping-Session")] string? sessionToken,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetOrderByIdQuery(id, sessionToken, User.GetUserId()),
            cancellationToken);

        return OkResponse(result);
    }
}
