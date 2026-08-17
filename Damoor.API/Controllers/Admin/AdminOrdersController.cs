using Asp.Versioning;
using Damoor.API.Controllers;
using Damoor.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

[Authorize(Policy = PolicyNames.AdminOnly)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/Admin/Orders")]
public sealed partial class AdminOrdersController : ApiBaseController
{
    private readonly ISender _sender;

    public AdminOrdersController(ISender sender)
    {
        _sender = sender;
    }
}
