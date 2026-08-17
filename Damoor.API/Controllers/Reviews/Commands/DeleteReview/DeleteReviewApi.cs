using Damoor.API.Extensions;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Reviews.Commands.DeleteReview;
using Damoor.Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Damoor.API.Controllers.Reviews;

public sealed partial class ReviewsController
{
    [HttpDelete("{id:int}")]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new DeleteReviewCommand(
                id,
                User.GetUserId()!.Value,
                User.IsInRole(RoleNames.Admin)),
            cancellationToken);

        return NoContentResponse("Review deleted successfully.");
    }
}
