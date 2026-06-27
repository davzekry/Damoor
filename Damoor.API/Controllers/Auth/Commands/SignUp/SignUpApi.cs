using Damoor.Application.Common.Models;
using Damoor.Application.Features.Authentication.Common;
using Damoor.Application.Features.Authentication.SignUp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Damoor.API.Controllers.Auth;

public sealed partial class AuthController
{
    [HttpPost("sign-up")]
    [EnableRateLimiting("strict")]
    [ProducesResponseType(
        typeof(ApiResponse<AuthResponse>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> SignUp(
        [FromBody] SignUpCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return CreatedResponse(result, "Account created successfully.");
    }
}
