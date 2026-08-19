using Asp.Versioning;
using Damoor.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Account;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed partial class AccountController : ApiBaseController
{
    private readonly ISender _sender;

    public AccountController(ISender sender)
    {
        _sender = sender;
    }
}
