using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Products.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Commands.CreateProductVariant;

public sealed class CreateProductVariantHandler
    : IRequestHandler<CreateProductVariantCommand, List<ProductVariantModel>>
{
    private readonly DamoorDbContext _db;

    public CreateProductVariantHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductVariantModel>> Handle(
        CreateProductVariantCommand request,
        CancellationToken cancellationToken)
    {
        var productExists = await _db.Products
            .AnyAsync(x => x.Id == request.ProductId, cancellationToken);

        if (!productExists)
            throw new NotFoundException("Product", request.ProductId);

        var variants = new List<ProductVariant>(request.Variants.Count);
        var seenSkus = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var seenOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in request.Variants)
        {
            var sku = item.SKU.Trim();
            var size = item.Size.Trim();
            var color = item.Color.Trim();

            if (!seenSkus.Add(sku))
            {
                throw new ConflictException(
                    $"Duplicate SKU '{sku}' in the request.");
            }

            if (!seenOptions.Add($"{size}|{color}"))
            {
                throw new ConflictException(
                    $"Duplicate size '{size}' and color '{color}' in the request.");
            }

            await EnsureUniqueVariantAsync(
                request.ProductId,
                sku,
                size,
                color,
                cancellationToken);

            variants.Add(new ProductVariant
            {
                ProductId = request.ProductId,
                SKU = sku,
                Size = size,
                Color = color,
                Price = item.Price,
                SalePrice = item.SalePrice,
                StockQuantity = item.StockQuantity,
                Images = item.Images
                    .Select(image => new ProductImage
                    {
                        ProductId = request.ProductId,
                        ImageUrl = image.ImageUrl.Trim(),
                        IsMain = image.IsMain
                    })
                    .ToList()
            });
        }

        _db.ProductVariants.AddRange(variants);
        await _db.SaveChangesAsync(cancellationToken);

        return variants.Select(ToModel).ToList();
    }

    private async Task EnsureUniqueVariantAsync(
        int productId,
        string sku,
        string size,
        string color,
        CancellationToken cancellationToken)
    {
        var duplicateSku = await _db.ProductVariants
            .AnyAsync(x => x.SKU == sku, cancellationToken);

        if (duplicateSku)
            throw new ConflictException("A product variant with this SKU already exists.");

        var duplicateOption = await _db.ProductVariants
            .AnyAsync(
                x => x.ProductId == productId &&
                     x.Size == size &&
                     x.Color == color,
                cancellationToken);

        if (duplicateOption)
        {
            throw new ConflictException(
                "A product variant with this size and color already exists.");
        }
    }

    private static ProductVariantModel ToModel(ProductVariant variant)
        => new()
        {
            Id = variant.Id,
            SKU = variant.SKU,
            Size = variant.Size,
            Color = variant.Color,
            Price = variant.Price,
            SalePrice = variant.SalePrice,
            StockQuantity = variant.StockQuantity,
            Images = variant.Images
                .OrderByDescending(i => i.IsMain)
                .ThenBy(i => i.Id)
                .Select(i => new ProductImageModel
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    IsMain = i.IsMain
                })
                .ToList()
        };
}
