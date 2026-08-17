using Damoor.Application.Common.Models;
using Damoor.Application.Features.Orders.Models;
using Damoor.Application.Features.Orders.Queries.AdminGetOrders;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminOrdersController
{
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<List<AdminOrderSummaryResult>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AdminOrderSummaryResult>>>> GetAll(
        [FromQuery] AdminGetOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return OkPaged(result, $"Found {result.TotalCount} order(s).");
    }
}
