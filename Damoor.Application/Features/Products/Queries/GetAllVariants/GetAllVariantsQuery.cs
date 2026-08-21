using MediatR;

namespace Damoor.Application.Features.Products.Queries.GetAllVariants;

public sealed record GetAllVariantsQuery : IRequest<List<GetAllVariantsResult>>;
