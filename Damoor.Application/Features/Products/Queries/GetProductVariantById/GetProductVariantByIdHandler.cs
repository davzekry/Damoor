using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Products.Models;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Queries.GetProductVariantById;

public sealed class GetProductVariantByIdHandler
    : IRequestHandler<GetProductVariantByIdQuery, GetProductVariantByIdResult>
{
    private readonly DamoorDbContext _db;

    public GetProductVariantByIdHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<GetProductVariantByIdResult> Handle(
        GetProductVariantByIdQuery request,
        CancellationToken cancellationToken)
    {
        var variant = await _db.ProductVariants
            .AsNoTracking()
            .Where(v => v.Id == request.Id)
            .Select(v => new GetProductVariantByIdResult
            {
                Id = v.Id,
                ProductId = v.ProductId,
                ProductName = v.Product.Name,
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
            .FirstOrDefaultAsync(cancellationToken);

        if (variant is null)
            throw new NotFoundException("ProductVariant", request.Id);

        return variant;
    }
}
