using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Commands.SetMainProductImage;
using Damoor.Application.Features.Products.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminProductImagesController
{
    [HttpPut("{id:int}/main")]
    [ProducesResponseType(
        typeof(ApiResponse<ProductImageModel>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProductImageModel>>> SetMain(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new SetMainProductImageCommand(id),
            cancellationToken);

        return OkResponse(result, "Main product image updated successfully.");
    }
}
