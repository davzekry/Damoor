using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Commands.UpdateProduct;
using Damoor.Application.Features.Products.Queries.GetProductById;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminProductsController
{
    [HttpPut("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<GetProductByIdResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<GetProductByIdResult>>> Update(
        int id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateProductCommand(
                id,
                request.Name,
                request.Description,
                request.CategoryId),
            cancellationToken);

        return OkResponse(result, "Product updated successfully.");
    }
}

public sealed record UpdateProductRequest(
    string Name,
    string Description,
    int CategoryId);
