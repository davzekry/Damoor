using Asp.Versioning;
using Damoor.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Auth;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/Auth")]
public sealed partial class AuthAccountController : ApiBaseController
{
    private readonly ISender _sender;

    public AuthAccountController(ISender sender)
    {
        _sender = sender;
    }
}
