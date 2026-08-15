using Damoor.Application.Common.Exceptions;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Commands.DeleteProduct;

public sealed class DeleteProductHandler
    : IRequestHandler<DeleteProductCommand>
{
    private readonly DamoorDbContext _db;

    public DeleteProductHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (product is null)
            throw new NotFoundException("Product", request.Id);

        _db.Products.Remove(product);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
