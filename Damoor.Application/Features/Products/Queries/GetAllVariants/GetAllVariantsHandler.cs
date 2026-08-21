using Damoor.Application.Features.Products.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Queries.GetAllVariants;

public sealed class GetAllVariantsHandler
    : IRequestHandler<GetAllVariantsQuery, List<GetAllVariantsResult>>
{
    private readonly DamoorDbContext _db;

    public GetAllVariantsHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public Task<List<GetAllVariantsResult>> Handle(
        GetAllVariantsQuery request,
        CancellationToken cancellationToken)
        => _db.ProductVariants
            .AsNoTracking()
            .OrderBy(v => v.Product.Name)
            .ThenBy(v => v.Size)
            .ThenBy(v => v.Color)
            .Select(v => new GetAllVariantsResult
            {
                Id = v.Id,
                ProductId = v.ProductId,
                ProductName = v.Product.Name,
                CategoryId = v.Product.CategoryId,
                CategoryName = v.Product.Category.Name,
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
            .ToListAsync(cancellationToken);
}
