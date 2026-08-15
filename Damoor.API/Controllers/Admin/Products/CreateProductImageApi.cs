using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Commands.CreateProductImage;
using Damoor.Application.Features.Products.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminProductsController
{
    [HttpPost("{productId:int}/images")]
    [ProducesResponseType(
        typeof(ApiResponse<ProductImageModel>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ProductImageModel>>> CreateImage(
        int productId,
        [FromBody] CreateProductImageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateProductImageCommand(
                productId,
                request.ImageUrl,
                request.IsMain),
            cancellationToken);

        return CreatedResponse(result, "Product image created successfully.");
    }
}

public sealed record CreateProductImageRequest(
    string ImageUrl,
    bool IsMain = false);
