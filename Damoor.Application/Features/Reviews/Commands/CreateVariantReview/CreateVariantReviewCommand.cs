using Damoor.Application.Features.Reviews.Models;
using MediatR;

namespace Damoor.Application.Features.Reviews.Commands.CreateVariantReview;

public sealed record CreateVariantReviewCommand(
    int ProductVariantId,
    int UserId,
    int Rating,
    string? Comment) : IRequest<ReviewResult>;
