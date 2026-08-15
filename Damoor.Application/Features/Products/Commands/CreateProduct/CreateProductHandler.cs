using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Products.Queries.GetProductById;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductHandler
    : IRequestHandler<CreateProductCommand, GetProductByIdResult>
{
    private readonly DamoorDbContext _db;
    private readonly ISender _sender;

    public CreateProductHandler(DamoorDbContext db, ISender sender)
    {
        _db = db;
        _sender = sender;
    }

    public async Task<GetProductByIdResult> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var categoryExists = await _db.Categories
            .AnyAsync(x => x.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new NotFoundException("Category", request.CategoryId);

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            CategoryId = request.CategoryId
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync(cancellationToken);

        return await _sender.Send(
            new GetProductByIdQuery(product.Id),
            cancellationToken);
    }
}
