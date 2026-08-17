using Damoor.Application.Common.Models;
using Damoor.Application.Features.Reviews.Models;
using Damoor.Application.Features.Reviews.Queries.GetProductReviews;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Products;

public sealed partial class ProductsController
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
            new GetProductReviewsQuery(id),
            cancellationToken);

        return OkResponse(result, $"Found {result.Count} review(s).");
    }
}
