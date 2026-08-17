using Asp.Versioning;
using Damoor.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Cart;

[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed partial class CartController : ApiBaseController
{
    private readonly ISender _sender;

    public CartController(ISender sender)
    {
        _sender = sender;
    }
}
