using Asp.Versioning;
using Damoor.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.ProductVariants;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ProductVariants/{productVariantId:int}/reviews")]
public sealed partial class ProductVariantReviewsController : ApiBaseController
{
    private readonly ISender _sender;

    public ProductVariantReviewsController(ISender sender)
    {
        _sender = sender;
    }
}
