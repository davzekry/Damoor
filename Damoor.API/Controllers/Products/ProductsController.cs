using MediatR;
using Microsoft.AspNetCore.Authorization;
using Damoor.API.Controllers;
using Asp.Versioning;
using Microsoft.AspNetCore.Components;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed partial class ProductsController : ApiBaseController
{
    private readonly ISender _sender;
    public ProductsController(ISender sender) => _sender = sender;
}