using Damoor.Application.Common.Models;
using Damoor.Application.Features.Products.Queries.GetAllVariants;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.ProductVariants;

public sealed partial class ProductVariantsController
{
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<List<GetAllVariantsResult>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetAllVariantsResult>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAllVariantsQuery(),
            cancellationToken);

        return OkResponse(result, $"Found {result.Count} variant(s).");
    }
}
