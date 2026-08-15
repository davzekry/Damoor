using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Commands.UpdateProductVariant;
using Damoor.Application.Features.Products.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminProductVariantsController
{
    [HttpPut("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<ProductVariantModel>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProductVariantModel>>> Update(
        int id,
        [FromBody] UpdateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateProductVariantCommand(
                id,
                request.SKU,
                request.Size,
                request.Color,
                request.Price,
                request.StockQuantity),
            cancellationToken);

        return OkResponse(result, "Product variant updated successfully.");
    }
}

public sealed record UpdateProductVariantRequest(
    string SKU,
    string Size,
    string Color,
    decimal Price,
    int StockQuantity);
