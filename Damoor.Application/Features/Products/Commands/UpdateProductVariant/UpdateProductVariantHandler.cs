using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Products.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Commands.UpdateProductVariant;

public sealed class UpdateProductVariantHandler
    : IRequestHandler<UpdateProductVariantCommand, ProductVariantModel>
{
    private readonly DamoorDbContext _db;

    public UpdateProductVariantHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<ProductVariantModel> Handle(
        UpdateProductVariantCommand request,
        CancellationToken cancellationToken)
    {
        var variant = await _db.ProductVariants
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (variant is null)
            throw new NotFoundException("ProductVariant", request.Id);

        var sku = request.SKU.Trim();
        var size = request.Size.Trim();
        var color = request.Color.Trim();

        await EnsureUniqueVariantAsync(
            variant.ProductId,
            sku,
            size,
            color,
            variant.Id,
            cancellationToken);

        variant.SKU = sku;
        variant.Size = size;
        variant.Color = color;
        variant.Price = request.Price;
        variant.SalePrice = request.SalePrice;
        variant.StockQuantity = request.StockQuantity;

        await _db.SaveChangesAsync(cancellationToken);

        return ToModel(variant);
    }

    private async Task EnsureUniqueVariantAsync(
        int productId,
        string sku,
        string size,
        string color,
        int exceptVariantId,
        CancellationToken cancellationToken)
    {
        var duplicateSku = await _db.ProductVariants
            .AnyAsync(
                x => x.SKU == sku && x.Id != exceptVariantId,
                cancellationToken);

        if (duplicateSku)
            throw new ConflictException("A product variant with this SKU already exists.");

        var duplicateOption = await _db.ProductVariants
            .AnyAsync(
                x => x.ProductId == productId &&
                     x.Size == size &&
                     x.Color == color &&
                     x.Id != exceptVariantId,
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
            StockQuantity = variant.StockQuantity
        };
}
