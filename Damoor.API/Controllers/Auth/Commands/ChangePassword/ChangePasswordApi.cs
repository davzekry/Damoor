using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Authentication.ChangePassword;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Auth;

public sealed partial class AuthAccountController
{
    [HttpPost("change-password")]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ChangePasswordCommand(
                User.GetUserId()!.Value,
                request.CurrentPassword,
                request.NewPassword),
            cancellationToken);

        return NoContentResponse("Password changed successfully.");
    }
}

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
