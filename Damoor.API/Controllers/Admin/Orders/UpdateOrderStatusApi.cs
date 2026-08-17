using Damoor.Application.Common.Models;
using Damoor.Application.Features.Orders.Commands.UpdateOrderStatus;
using Damoor.Application.Features.Orders.Models;
using Damoor.Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminOrdersController
{
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(
        typeof(ApiResponse<AdminOrderDetailsResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AdminOrderDetailsResult>>> UpdateStatus(
        int id,
        [FromBody] UpdateOrderStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateOrderStatusCommand(id, request.Status),
            cancellationToken);

        return OkResponse(result, "Order status updated successfully.");
    }
}

public sealed record UpdateOrderStatusRequest(OrderStatus Status);
