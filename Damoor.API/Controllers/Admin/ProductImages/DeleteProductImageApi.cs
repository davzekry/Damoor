using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Commands.DeleteProductImage;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminProductImagesController
{
    [HttpDelete("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteProductImageCommand(id), cancellationToken);
        return NoContentResponse("Product image deleted successfully.");
    }
}
