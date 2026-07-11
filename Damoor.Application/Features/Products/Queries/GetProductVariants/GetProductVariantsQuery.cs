using MediatR;

namespace Damoor.Application.Features.Products.Queries.GetProductVariants;

public sealed record GetProductVariantsQuery(int ProductId)
    : IRequest<List<GetProductVariantResult>>;
