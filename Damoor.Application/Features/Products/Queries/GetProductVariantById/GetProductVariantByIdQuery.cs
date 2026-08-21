using MediatR;

namespace Damoor.Application.Features.Products.Queries.GetProductVariantById;

public sealed record GetProductVariantByIdQuery(int Id)
    : IRequest<GetProductVariantByIdResult>;
