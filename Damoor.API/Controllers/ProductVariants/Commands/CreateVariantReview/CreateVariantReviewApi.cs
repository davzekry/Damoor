using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Reviews.Commands.CreateVariantReview;
using Damoor.Application.Features.Reviews.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.ProductVariants;

public sealed partial class ProductVariantReviewsController
{
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<ReviewResult>),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ReviewResult>>> Create(
        int productVariantId,
        [FromBody] CreateVariantReviewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateVariantReviewCommand(
                productVariantId,
                User.GetUserId()!.Value,
                request.Rating,
                request.Comment),
            cancellationToken);

        return CreatedResponse(result, "Review created successfully.");
    }
}

public sealed record CreateVariantReviewRequest(int Rating, string? Comment);
