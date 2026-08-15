using Damoor.Application.Common.Exceptions;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryHandler
    : IRequestHandler<DeleteCategoryCommand>
{
    private readonly DamoorDbContext _db;

    public DeleteCategoryHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _db.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (category is null)
            throw new NotFoundException("Category", request.Id);

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
