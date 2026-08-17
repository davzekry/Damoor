using Asp.Versioning;
using Damoor.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.ShoppingSessions;

[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed partial class ShoppingSessionsController : ApiBaseController
{
    private readonly ISender _sender;

    public ShoppingSessionsController(ISender sender)
    {
        _sender = sender;
    }
}
