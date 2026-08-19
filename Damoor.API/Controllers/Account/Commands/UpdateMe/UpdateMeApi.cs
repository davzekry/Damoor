using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Account.Commands.UpdateMe;
using Damoor.Application.Features.Account.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Account;

public sealed partial class AccountController
{
    [HttpPut("me")]
    [ProducesResponseType(
        typeof(ApiResponse<AccountResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AccountResult>>> UpdateMe(
        [FromBody] UpdateMeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateMeCommand(User.GetUserId()!.Value, request.FullName),
            cancellationToken);

        return OkResponse(result, "Account updated successfully.");
    }
}

public sealed record UpdateMeRequest(string FullName);
