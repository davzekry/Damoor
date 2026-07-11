using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Queries.GetAllProducts;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Products;

public sealed partial class ProductsController
{
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<List<GetAllProductsResult>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetAllProductsResult>>>> GetAll(
        [FromQuery] GetAllProductsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return OkPaged(result, $"Found {result.TotalCount} product(s).");
    }
}
