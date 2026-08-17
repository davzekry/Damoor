using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Reviews.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Reviews.Queries.GetProductReviews;

public sealed class GetProductReviewsHandler
    : IRequestHandler<GetProductReviewsQuery, List<ReviewResult>>
{
    private readonly DamoorDbContext _db;

    public GetProductReviewsHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<List<ReviewResult>> Handle(
        GetProductReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var productExists = await _db.Products
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.ProductId, cancellationToken);

        if (!productExists)
            throw new NotFoundException("Product", request.ProductId);

        return await (
            from r in _db.Reviews.AsNoTracking()
            join u in _db.Users.AsNoTracking() on r.UserId equals u.Id
            where r.ProductId == request.ProductId
            orderby r.CreatedAt descending
            select new ReviewResult
            {
                Id = r.Id,
                ProductId = r.ProductId,
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
