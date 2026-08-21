using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Commands.CreateProductVariant;
using Damoor.Application.Features.Products.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminProductsController
{
    [HttpPost("{productId:int}/variants")]
    [ProducesResponseType(
        typeof(ApiResponse<List<ProductVariantModel>>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<List<ProductVariantModel>>>> CreateVariant(
        int productId,
        [FromBody] CreateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateProductVariantCommand(
                productId,
                request.Variants
                    .Select(v => new CreateProductVariantItem(
                        v.SKU,
                        v.Size,
                        v.Color,
                        v.Price,
                        v.SalePrice,
                        v.StockQuantity,
                        v.Images
                            .Select(i => new CreateProductVariantImageItem(
                                i.ImageUrl,
                                i.IsMain))
                            .ToList()))
                    .ToList()),
            cancellationToken);

        return CreatedResponse(
            result,
            $"{result.Count} product variant(s) created successfully.");
    }
}

public sealed record CreateProductVariantRequest(
    List<CreateProductVariantRequestItem> Variants);

public sealed record CreateProductVariantRequestItem(
    string SKU,
    string Size,
    string Color,
    decimal Price,
    decimal? SalePrice,
    int StockQuantity,
    List<CreateProductVariantRequestImage> Images);

public sealed record CreateProductVariantRequestImage(
    string ImageUrl,
    bool IsMain = false);
