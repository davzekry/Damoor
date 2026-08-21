using Damoor.Application.Features.Reviews.Models;
using MediatR;

namespace Damoor.Application.Features.Reviews.Commands.UpdateVariantReview;

public sealed record UpdateVariantReviewCommand(
    int ProductVariantId,
    int Id,
    int UserId,
    int Rating,
    string? Comment) : IRequest<ReviewResult>;
