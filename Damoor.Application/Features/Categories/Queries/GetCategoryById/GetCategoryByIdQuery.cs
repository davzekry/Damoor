using MediatR;

namespace Damoor.Application.Features.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(int Id)
    : IRequest<GetCategoryByIdResult>;
