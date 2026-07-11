using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Categories.Queries.GetAllCategories;

public sealed class GetAllCategoriesHandler
    : IRequestHandler<GetAllCategoriesQuery, List<GetAllCategoriesResult>>
{
    private readonly DamoorDbContext _db;

    public GetAllCategoriesHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public Task<List<GetAllCategoriesResult>> Handle(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
        => _db.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new GetAllCategoriesResult
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ProductCount = x.Products.Count
            })
            .ToListAsync(cancellationToken);
}
