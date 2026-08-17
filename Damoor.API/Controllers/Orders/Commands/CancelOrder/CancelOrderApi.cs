using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Orders.Commands.CancelOrder;
using Damoor.Application.Features.Orders.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Orders;

public sealed partial class OrdersController
{
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(
        typeof(ApiResponse<OrderDetailsResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<OrderDetailsResult>>> Cancel(
        int id,
        [FromHeader(Name = "X-Shopping-Session")] string? sessionToken,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CancelOrderCommand(id, sessionToken, User.GetUserId()),
            cancellationToken);

        return OkResponse(result, "Order cancelled successfully.");
    }
}
