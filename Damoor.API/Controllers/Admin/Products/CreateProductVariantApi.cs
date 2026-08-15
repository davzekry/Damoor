using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Commands.CreateProductVariant;
using Damoor.Application.Features.Products.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminProductsController
{
    [HttpPost("{productId:int}/variants")]
    [ProducesResponseType(
        typeof(ApiResponse<ProductVariantModel>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ProductVariantModel>>> CreateVariant(
        int productId,
        [FromBody] CreateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateProductVariantCommand(
                productId,
                request.SKU,
                request.Size,
                request.Color,
                request.Price,
                request.StockQuantity),
            cancellationToken);

        return CreatedResponse(result, "Product variant created successfully.");
    }
}

public sealed record CreateProductVariantRequest(
    string SKU,
    string Size,
    string Color,
    decimal Price,
    int StockQuantity);
