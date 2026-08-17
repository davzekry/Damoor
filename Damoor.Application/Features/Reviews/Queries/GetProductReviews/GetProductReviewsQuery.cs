using Damoor.Application.Features.Reviews.Models;
using MediatR;

namespace Damoor.Application.Features.Reviews.Queries.GetProductReviews;

public sealed record GetProductReviewsQuery(int ProductId) : IRequest<List<ReviewResult>>;
