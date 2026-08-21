using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Reviews.Commands.UpdateVariantReview;
using Damoor.Application.Features.Reviews.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.ProductVariants;

public sealed partial class ProductVariantReviewsController
{
    [HttpPut("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<ReviewResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReviewResult>>> Update(
        int productVariantId,
        int id,
        [FromBody] UpdateVariantReviewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateVariantReviewCommand(
                productVariantId,
                id,
                User.GetUserId()!.Value,
                request.Rating,
                request.Comment),
            cancellationToken);

        return OkResponse(result, "Review updated successfully.");
    }
}

public sealed record UpdateVariantReviewRequest(int Rating, string? Comment);
