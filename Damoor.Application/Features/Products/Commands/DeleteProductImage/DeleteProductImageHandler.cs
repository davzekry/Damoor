using Damoor.Application.Common.Exceptions;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Commands.DeleteProductImage;

public sealed class DeleteProductImageHandler
    : IRequestHandler<DeleteProductImageCommand>
{
    private readonly DamoorDbContext _db;

    public DeleteProductImageHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task Handle(
        DeleteProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var image = await _db.ProductImages
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (image is null)
            throw new NotFoundException("ProductImage", request.Id);

        _db.ProductImages.Remove(image);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
