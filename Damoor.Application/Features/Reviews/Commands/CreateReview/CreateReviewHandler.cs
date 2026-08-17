using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Reviews.Common;
using Damoor.Application.Features.Reviews.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Reviews.Commands.CreateReview;

public sealed class CreateReviewHandler
    : IRequestHandler<CreateReviewCommand, ReviewResult>
{
    private readonly DamoorDbContext _db;

    public CreateReviewHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<ReviewResult> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken)
    {
        var productExists = await _db.Products
            .AnyAsync(x => x.Id == request.ProductId, cancellationToken);

        if (!productExists)
            throw new NotFoundException("Product", request.ProductId);

        var alreadyReviewed = await _db.Reviews
            .AnyAsync(
                x => x.ProductId == request.ProductId && x.UserId == request.UserId,
                cancellationToken);

        if (alreadyReviewed)
            throw new ConflictException("You have already reviewed this product.");

        var review = new Review
        {
            ProductId = request.ProductId,
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
