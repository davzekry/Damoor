using Damoor.Application.Common.Models;
using Damoor.Application.Features.Orders.Models;
using Damoor.Application.Features.Orders.Queries.AdminGetOrderById;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminOrdersController
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<AdminOrderDetailsResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AdminOrderDetailsResult>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new AdminGetOrderByIdQuery(id),
            cancellationToken);

        return OkResponse(result);
    }
}
