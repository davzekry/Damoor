using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Products.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Commands.SetMainProductImage;

public sealed class SetMainProductImageHandler
    : IRequestHandler<SetMainProductImageCommand, ProductImageModel>
{
    private readonly DamoorDbContext _db;

    public SetMainProductImageHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<ProductImageModel> Handle(
        SetMainProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var image = await _db.ProductImages
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (image is null)
            throw new NotFoundException("ProductImage", request.Id);

        await _db.ProductImages
            .Where(x => x.ProductId == image.ProductId && x.Id != image.Id)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(x => x.IsMain, false),
                cancellationToken);

        image.IsMain = true;
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
