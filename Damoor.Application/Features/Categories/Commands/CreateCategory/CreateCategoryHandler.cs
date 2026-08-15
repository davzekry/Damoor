using Damoor.Application.Common.Exceptions;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Damoor.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryHandler
    : IRequestHandler<CreateCategoryCommand, CreateCategoryResult>
{
    private readonly DamoorDbContext _db;

    public CreateCategoryHandler(DamoorDbContext db)
    {
        _db = db;
    }

    public async Task<CreateCategoryResult> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var exists = await _db.Categories
            .AnyAsync(x => x.Name == name, cancellationToken);

        if (exists)
            throw new ConflictException("A category with this name already exists.");

        var category = new Category
        {
            Name = name,
            Description = request.Description?.Trim()
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync(cancellationToken);

        return new CreateCategoryResult
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}
