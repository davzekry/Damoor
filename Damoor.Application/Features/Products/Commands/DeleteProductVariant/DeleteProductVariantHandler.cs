using Damoor.Application.Common.Exceptions;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Commands.DeleteProductVariant;

public sealed class DeleteProductVariantHandler
    : IRequestHandler<DeleteProductVariantCommand>
{
    private readonly DamoorDbContext _db;

    public DeleteProductVariantHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task Handle(
        DeleteProductVariantCommand request,
        CancellationToken cancellationToken)
    {
        var variant = await _db.ProductVariants
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (variant is null)
            throw new NotFoundException("ProductVariant", request.Id);

        _db.ProductVariants.Remove(variant);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
