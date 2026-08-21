using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Queries.GetProductVariantById;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.ProductVariants;

public sealed partial class ProductVariantsController
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<GetProductVariantByIdResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<GetProductVariantByIdResult>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetProductVariantByIdQuery(id),
            cancellationToken);

        return OkResponse(result);
    }
}
