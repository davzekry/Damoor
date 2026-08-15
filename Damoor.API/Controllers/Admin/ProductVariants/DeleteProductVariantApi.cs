using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Commands.DeleteProductVariant;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Admin;

public sealed partial class AdminProductVariantsController
{
    [HttpDelete("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteProductVariantCommand(id), cancellationToken);
        return NoContentResponse("Product variant deleted successfully.");
    }
}
