using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Reviews.Commands.UpdateReview;
using Damoor.Application.Features.Reviews.Models;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Reviews;

public sealed partial class ReviewsController
{
    [HttpPut("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<ReviewResult>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReviewResult>>> Update(
        int id,
        [FromBody] UpdateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateReviewCommand(
                id,
                User.GetUserId()!.Value,
                request.Rating,
                request.Comment),
            cancellationToken);

        return OkResponse(result, "Review updated successfully.");
    }
}

public sealed record UpdateReviewRequest(int Rating, string? Comment);
