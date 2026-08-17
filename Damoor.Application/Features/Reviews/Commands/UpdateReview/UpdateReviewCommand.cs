using Damoor.Application.Features.Reviews.Models;
using MediatR;

namespace Damoor.Application.Features.Reviews.Commands.UpdateReview;

public sealed record UpdateReviewCommand(
    int Id,
    int UserId,
    int Rating,
    string? Comment) : IRequest<ReviewResult>;
