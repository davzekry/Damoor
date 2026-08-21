using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Reviews.Common;
using Damoor.Application.Features.Reviews.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Reviews.Commands.CreateVariantReview;

public sealed class CreateVariantReviewHandler
    : IRequestHandler<CreateVariantReviewCommand, ReviewResult>
{
    private readonly DamoorDbContext _db;

    public CreateVariantReviewHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<ReviewResult> Handle(
        CreateVariantReviewCommand request,
        CancellationToken cancellationToken)
    {
        var productId = await _db.ProductVariants
            .Where(x => x.Id == request.ProductVariantId)
            .Select(x => (int?)x.ProductId)
            .FirstOrDefaultAsync(cancellationToken);

        if (productId is null)
            throw new NotFoundException("ProductVariant", request.ProductVariantId);

        var alreadyReviewed = await _db.Reviews
            .AnyAsync(
                x => x.ProductVariantId == request.ProductVariantId &&
                     x.UserId == request.UserId,
                cancellationToken);

        if (alreadyReviewed)
            throw new ConflictException("You have already reviewed this variant.");

        var review = new Review
        {
            ProductId = productId.Value,
            ProductVariantId = request.ProductVariantId,
            UserId = request.UserId,
            Rating = request.Rating,
            Comment = request.Comment?.Trim()
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync(cancellationToken);

        var userName = await _db.Users
            .AsNoTracking()
            .Where(x => x.Id == request.UserId)
            .Select(x => x.FullName)
            .FirstAsync(cancellationToken);

        return ReviewMapper.ToResult(review, userName);
    }
}
