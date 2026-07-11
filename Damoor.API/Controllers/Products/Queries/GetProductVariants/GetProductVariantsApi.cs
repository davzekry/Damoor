using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Queries.GetProductVariants;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Products;

public sealed partial class ProductsController
{
    [HttpGet("{id:int}/variants")]
    [ProducesResponseType(
        typeof(ApiResponse<List<GetProductVariantResult>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetProductVariantResult>>>> GetVariants(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetProductVariantsQuery(id),
            cancellationToken);

        return OkResponse(result, $"Found {result.Count} variant(s).");
    }
}
