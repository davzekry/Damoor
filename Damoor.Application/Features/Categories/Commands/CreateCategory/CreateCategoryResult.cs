namespace Damoor.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
