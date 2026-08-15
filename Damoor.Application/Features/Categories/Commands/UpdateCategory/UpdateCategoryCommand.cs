using MediatR;

namespace Damoor.Application.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    int Id,
    string Name,
    string? Description) : IRequest<UpdateCategoryResult>;
