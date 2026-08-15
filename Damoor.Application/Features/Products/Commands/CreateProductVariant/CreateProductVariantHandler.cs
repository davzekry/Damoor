using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Products.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Commands.CreateProductVariant;

public sealed class CreateProductVariantHandler
    : IRequestHandler<CreateProductVariantCommand, ProductVariantModel>
{
    private readonly DamoorDbContext _db;

    public CreateProductVariantHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<ProductVariantModel> Handle(
        CreateProductVariantCommand request,
        CancellationToken cancellationToken)
    {
        var productExists = await _db.Products
            .AnyAsync(x => x.Id == request.ProductId, cancellationToken);

        if (!productExists)
            throw new NotFoundException("Product", request.ProductId);

        var sku = request.SKU.Trim();
        var size = request.Size.Trim();
        var color = request.Color.Trim();

        await EnsureUniqueVariantAsync(
            request.ProductId,
            sku,
            size,
            color,
            exceptVariantId: null,
            cancellationToken);

        var variant = new ProductVariant
        {
            ProductId = request.ProductId,
            SKU = sku,
            Size = size,
            Color = color,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        _db.ProductVariants.Add(variant);
        await _db.SaveChangesAsync(cancellationToken);

        return ToModel(variant);
    }

    private async Task EnsureUniqueVariantAsync(
        int productId,
        string sku,
        string size,
        string color,
        int? exceptVariantId,
        CancellationToken cancellationToken)
    {
        var duplicateSku = await _db.ProductVariants
            .AnyAsync(
                x => x.SKU == sku &&
                     (!exceptVariantId.HasValue || x.Id != exceptVariantId.Value),
                cancellationToken);

        if (duplicateSku)
            throw new ConflictException("A product variant with this SKU already exists.");

        var duplicateOption = await _db.ProductVariants
            .AnyAsync(
                x => x.ProductId == productId &&
                     x.Size == size &&
                     x.Color == color &&
                     (!exceptVariantId.HasValue || x.Id != exceptVariantId.Value),
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
            StockQuantity = variant.StockQuantity
        };
}
