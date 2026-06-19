using Damoor.Application.Common.Models;
using Damoor.Application.Features.Authentication.Common;
using Damoor.Application.Features.Authentication.SignIn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Damoor.API.Controllers.Auth;

public sealed partial class AuthController
{
    [HttpPost("sign-in")]
    [EnableRateLimiting("strict")]
    [ProducesResponseType(
        typeof(ApiResponse<AuthResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> SignIn(
        [FromBody] SignInCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return OkResponse(result, "Signed in successfully.");
    }
}
