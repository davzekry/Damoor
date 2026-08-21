using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Reviews.Common;
using Damoor.Application.Features.Reviews.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Reviews.Commands.UpdateVariantReview;

public sealed class UpdateVariantReviewHandler
    : IRequestHandler<UpdateVariantReviewCommand, ReviewResult>
{
    private readonly DamoorDbContext _db;

    public UpdateVariantReviewHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<ReviewResult> Handle(
        UpdateVariantReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await _db.Reviews
            .FirstOrDefaultAsync(
                x => x.Id == request.Id &&
                     x.ProductVariantId == request.ProductVariantId,
                cancellationToken);

        if (review is null)
            throw new NotFoundException("Review", request.Id);

        if (review.UserId != request.UserId)
            throw new UnauthorizedException("You are not authorized to modify this review.");

        review.Rating = request.Rating;
        review.Comment = request.Comment?.Trim();
        await _db.SaveChangesAsync(cancellationToken);

        var userName = await _db.Users
            .AsNoTracking()
            .Where(x => x.Id == review.UserId)
            .Select(x => x.FullName)
            .FirstAsync(cancellationToken);

        return ReviewMapper.ToResult(review, userName);
    }
}
