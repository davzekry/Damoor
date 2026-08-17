using Damoor.Application.Features.Reviews.Models;
using MediatR;

namespace Damoor.Application.Features.Reviews.Commands.CreateReview;

public sealed record CreateReviewCommand(
    int ProductId,
    int UserId,
    int Rating,
    string? Comment) : IRequest<ReviewResult>;
