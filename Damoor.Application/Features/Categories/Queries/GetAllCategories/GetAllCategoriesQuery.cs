using MediatR;

namespace Damoor.Application.Features.Categories.Queries.GetAllCategories;

public sealed record GetAllCategoriesQuery
    : IRequest<List<GetAllCategoriesResult>>;
