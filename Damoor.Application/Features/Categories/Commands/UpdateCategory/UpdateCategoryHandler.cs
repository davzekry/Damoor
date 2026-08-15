using Damoor.Application.Common.Exceptions;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryHandler
    : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResult>
{
    private readonly DamoorDbContext _db;

    public UpdateCategoryHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<UpdateCategoryResult> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _db.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (category is null)
            throw new NotFoundException("Category", request.Id);

        var name = request.Name.Trim();
        var duplicate = await _db.Categories
            .AnyAsync(x => x.Id != request.Id && x.Name == name, cancellationToken);

        if (duplicate)
            throw new ConflictException("A category with this name already exists.");

        category.Name = name;
        category.Description = request.Description?.Trim();

        await _db.SaveChangesAsync(cancellationToken);

        return new UpdateCategoryResult
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}
