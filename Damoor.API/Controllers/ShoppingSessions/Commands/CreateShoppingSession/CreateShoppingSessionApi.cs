using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.ShoppingSessions.Commands.CreateShoppingSession;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.ShoppingSessions;

public sealed partial class ShoppingSessionsController
{
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<CreateShoppingSessionResult>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CreateShoppingSessionResult>>> Create(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateShoppingSessionCommand(User.GetUserId()),
            cancellationToken);

        return CreatedResponse(result, "Shopping session created successfully.");
    }
}
