using Asp.Versioning;
using Damoor.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Products;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/Products/{productId:int}/reviews")]
public sealed partial class ProductReviewsController : ApiBaseController
{
    private readonly ISender _sender;

    public ProductReviewsController(ISender sender)
    {
        _sender = sender;
    }
}
