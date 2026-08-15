using MediatR;

namespace Damoor.Application.Features.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(int Id) : IRequest;
