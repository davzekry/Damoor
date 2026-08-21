using Damoor.Application.Features.Reviews.Models;
using MediatR;

namespace Damoor.Application.Features.Reviews.Queries.GetVariantReviews;

public sealed record GetVariantReviewsQuery(int ProductVariantId)
    : IRequest<List<ReviewResult>>;
