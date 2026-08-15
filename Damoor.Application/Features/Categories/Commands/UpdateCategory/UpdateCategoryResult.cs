namespace Damoor.Application.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
