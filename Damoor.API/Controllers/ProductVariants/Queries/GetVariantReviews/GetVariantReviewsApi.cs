using Damoor.Application.Common.Models;
using Damoor.Application.Features.Reviews.Models;
using Damoor.Application.Features.Reviews.Queries.GetVariantReviews;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.ProductVariants;

public sealed partial class ProductVariantsController
{
    [HttpGet("{id:int}/reviews")]
    [ProducesResponseType(
        typeof(ApiResponse<List<ReviewResult>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ReviewResult>>>> GetReviews(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetVariantReviewsQuery(id),
            cancellationToken);

        return OkResponse(result, $"Found {result.Count} review(s).");
    }
}
