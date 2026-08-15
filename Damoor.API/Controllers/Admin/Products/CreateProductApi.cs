using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Commands.CreateProduct;
using Damoor.Application.Features.Products.Queries.GetProductById;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminProductsController
{
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<GetProductByIdResult>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<GetProductByIdResult>>> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return CreatedResponse(result, "Product created successfully.");
    }
}
