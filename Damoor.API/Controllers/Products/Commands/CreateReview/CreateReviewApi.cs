using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Reviews.Commands.CreateReview;
using Damoor.Application.Features.Reviews.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Products;

public sealed partial class ProductReviewsController
{
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<ReviewResult>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ReviewResult>>> Create(
        int productId,
        [FromBody] CreateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateReviewCommand(
                productId,
                User.GetUserId()!.Value,
                request.Rating,
                request.Comment),
            cancellationToken);

        return CreatedResponse(result, "Review created successfully.");
    }
}

public sealed record CreateReviewRequest(int Rating, string? Comment);
