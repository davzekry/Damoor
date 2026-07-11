using MediatR;

namespace Damoor.Application.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(int Id)
    : IRequest<GetProductByIdResult>;
