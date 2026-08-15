using Asp.Versioning;
using Damoor.API.Controllers;
using Damoor.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

[Authorize(Policy = PolicyNames.AdminOnly)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/Admin/ProductVariants")]
public sealed partial class AdminProductVariantsController : ApiBaseController
{
    private readonly ISender _sender;

    public AdminProductVariantsController(ISender sender)
    {
        _sender = sender;
    }
}
