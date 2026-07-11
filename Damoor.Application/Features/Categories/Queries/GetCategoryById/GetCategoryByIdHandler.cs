using Damoor.Application.Common.Exceptions;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdHandler
    : IRequestHandler<GetCategoryByIdQuery, GetCategoryByIdResult>
{
    private readonly DamoorDbContext _db;

    public GetCategoryByIdHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<GetCategoryByIdResult> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _db.Categories
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetCategoryByIdResult
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ProductCount = x.Products.Count
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
            throw new NotFoundException("Category", request.Id);

        return category;
    }
}
