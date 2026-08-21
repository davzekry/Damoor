using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Products.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Commands.CreateProductImage;

public sealed class CreateProductImageHandler
    : IRequestHandler<CreateProductImageCommand, ProductImageModel>
{
    private readonly DamoorDbContext _db;

    public CreateProductImageHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<ProductImageModel> Handle(
        CreateProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var productExists = await _db.Products
            .AnyAsync(x => x.Id == request.ProductId, cancellationToken);

        if (!productExists)
            throw new NotFoundException("Product", request.ProductId);

        var hasImages = await _db.ProductImages
            .AnyAsync(
                x => x.ProductId == request.ProductId &&
                     x.ProductVariantId == null,
                cancellationToken);

        var shouldBeMain = request.IsMain || !hasImages;

        if (shouldBeMain)
        {
            await _db.ProductImages
                .Where(x =>
                    x.ProductId == request.ProductId &&
                    x.ProductVariantId == null &&
                    x.IsMain)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(x => x.IsMain, false),
                    cancellationToken);
        }

        var image = new ProductImage
        {
            ProductId = request.ProductId,
            ImageUrl = request.ImageUrl.Trim(),
            IsMain = shouldBeMain
        };

        _db.ProductImages.Add(image);
        await _db.SaveChangesAsync(cancellationToken);

        return ToModel(image);
    }

    private static ProductImageModel ToModel(ProductImage image)
        => new()
        {
            Id = image.Id,
            ImageUrl = image.ImageUrl,
            IsMain = image.IsMain
        };
}
