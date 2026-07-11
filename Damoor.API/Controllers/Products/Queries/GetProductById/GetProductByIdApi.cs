using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Queries.GetProductById;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Products;

public sealed partial class ProductsController
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<GetProductByIdResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<GetProductByIdResult>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        return OkResponse(result);
    }
}
