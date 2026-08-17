using Damoor.Application.Features.Reviews.Models;
using Damoor.Domain.Entities;

namespace Damoor.Application.Features.Reviews.Common;

internal static class ReviewMapper
{
    public static ReviewResult ToResult(Review review, string userName)
        => new()
        {
            Id = review.Id,
            ProductId = review.ProductId,
            UserId = review.UserId,
            UserName = userName,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
}
