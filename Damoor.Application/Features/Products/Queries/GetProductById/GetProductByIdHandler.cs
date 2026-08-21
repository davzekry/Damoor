using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Products.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdHandler
    : IRequestHandler<GetProductByIdQuery, GetProductByIdResult>
{
    private readonly DamoorDbContext _db;

    public GetProductByIdHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<GetProductByIdResult> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new GetProductByIdResult
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                MinPrice = p.Variants
                    .Select(v => (decimal?)v.Price)
                    .Min(),
                TotalStockQuantity = p.Variants
                    .Select(v => (int?)v.StockQuantity)
                    .Sum() ?? 0,
                AverageRating = p.Reviews.Any(r => r.ProductVariantId == null)
                    ? p.Reviews
                        .Where(r => r.ProductVariantId == null)
                        .Average(r => (double?)r.Rating)
                    : null,
                ReviewCount = p.Reviews.Count(r => r.ProductVariantId == null),
                Images = p.Images
                    .Where(i => i.ProductVariantId == null)
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.Id)
                    .Select(i => new ProductImageModel
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        IsMain = i.IsMain
                    })
                    .ToList(),
                Variants = p.Variants
                    .OrderBy(v => v.Size)
                    .ThenBy(v => v.Color)
                    .Select(v => new ProductVariantModel
                    {
                        Id = v.Id,
                        SKU = v.SKU,
                        Size = v.Size,
                        Color = v.Color,
                        Price = v.Price,
                        SalePrice = v.SalePrice,
                        StockQuantity = v.StockQuantity,
                        Images = v.Images
                            .OrderByDescending(i => i.IsMain)
                            .ThenBy(i => i.Id)
                            .Select(i => new ProductImageModel
                            {
                                Id = i.Id,
                                ImageUrl = i.ImageUrl,
                                IsMain = i.IsMain
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            throw new NotFoundException("Product", request.Id);

        return product;
    }
}
