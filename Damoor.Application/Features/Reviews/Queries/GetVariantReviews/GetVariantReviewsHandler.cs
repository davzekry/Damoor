using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Reviews.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Reviews.Queries.GetVariantReviews;

public sealed class GetVariantReviewsHandler
    : IRequestHandler<GetVariantReviewsQuery, List<ReviewResult>>
{
    private readonly DamoorDbContext _db;

    public GetVariantReviewsHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<List<ReviewResult>> Handle(
        GetVariantReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var variantExists = await _db.ProductVariants
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.ProductVariantId, cancellationToken);

        if (!variantExists)
            throw new NotFoundException("ProductVariant", request.ProductVariantId);

        return await (
            from r in _db.Reviews.AsNoTracking()
            join u in _db.Users.AsNoTracking() on r.UserId equals u.Id
            where r.ProductVariantId == request.ProductVariantId
            orderby r.CreatedAt descending
            select new ReviewResult
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductVariantId = r.ProductVariantId,
                UserId = r.UserId,
                UserName = u.FullName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
