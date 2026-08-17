using MediatR;

namespace Damoor.Application.Features.Reviews.Commands.DeleteReview;

public sealed record DeleteReviewCommand(
    int Id,
    int UserId,
    bool IsAdmin) : IRequest;
