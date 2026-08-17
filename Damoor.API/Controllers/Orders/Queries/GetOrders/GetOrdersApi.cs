using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Orders.Models;
using Damoor.Application.Features.Orders.Queries.GetOrders;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Orders;

public sealed partial class OrdersController
{
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<List<OrderSummaryResult>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<OrderSummaryResult>>>> GetAll(
        [FromHeader(Name = "X-Shopping-Session")] string? sessionToken,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetOrdersQuery(sessionToken, User.GetUserId()),
            cancellationToken);

        return OkResponse(result, $"Found {result.Count} order(s).");
    }
}
