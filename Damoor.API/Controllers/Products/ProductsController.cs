using MediatR;
using Microsoft.AspNetCore.Authorization;
using Damoor.API.Controllers;

//[Authorize]
public sealed partial class ProductsController : ApiBaseController
{
    private readonly ISender _sender;
    public ProductsController(ISender sender) => _sender = sender;
}