using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Account.Models;
using Damoor.Application.Features.Account.Queries.GetMe;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Account;

public sealed partial class AccountController
{
    [HttpGet("me")]
    [ProducesResponseType(
        typeof(ApiResponse<AccountResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AccountResult>>> GetMe(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetMeQuery(User.GetUserId()!.Value),
            cancellationToken);

        return OkResponse(result);
    }
}
