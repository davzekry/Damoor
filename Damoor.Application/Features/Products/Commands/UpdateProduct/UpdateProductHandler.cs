using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Products.Queries.GetProductById;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductHandler
    : IRequestHandler<UpdateProductCommand, GetProductByIdResult>
{
    private readonly DamoorDbContext _db;
    private readonly ISender _sender;

    public UpdateProductHandler(DamoorDbContext db, ISender sender)
    {
        _db = db;
        _sender = sender;
    }

    public async Task<GetProductByIdResult> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (product is null)
            throw new NotFoundException("Product", request.Id);

        var categoryExists = await _db.Categories
            .AnyAsync(x => x.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new NotFoundException("Category", request.CategoryId);

        product.Name = request.Name.Trim();
        product.Description = request.Description.Trim();
        product.CategoryId = request.CategoryId;

        await _db.SaveChangesAsync(cancellationToken);

        return await _sender.Send(
            new GetProductByIdQuery(product.Id),
            cancellationToken);
    }
}
