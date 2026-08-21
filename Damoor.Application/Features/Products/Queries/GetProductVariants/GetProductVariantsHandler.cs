using Damoor.Application.Common.Exceptions;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Queries.GetProductVariants;

public sealed class GetProductVariantsHandler
    : IRequestHandler<GetProductVariantsQuery, List<GetProductVariantResult>>
{
    private readonly DamoorDbContext _db;

    public GetProductVariantsHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<List<GetProductVariantResult>> Handle(
        GetProductVariantsQuery request,
        CancellationToken cancellationToken)
    {
        var productExists = await _db.Products
            .AsNoTracking()
            .AnyAsync(p => p.Id == request.ProductId, cancellationToken);

        if (!productExists)
            throw new NotFoundException("Product", request.ProductId);

        return await _db.ProductVariants
            .AsNoTracking()
            .Where(v => v.ProductId == request.ProductId)
            .OrderBy(v => v.Size)
            .ThenBy(v => v.Color)
            .Select(v => new GetProductVariantResult
            {
                Id = v.Id,
                ProductId = v.ProductId,
                SKU = v.SKU,
                Size = v.Size,
                Color = v.Color,
                Price = v.Price,
                SalePrice = v.SalePrice,
                StockQuantity = v.StockQuantity
            })
            .ToListAsync(cancellationToken);
    }
}
