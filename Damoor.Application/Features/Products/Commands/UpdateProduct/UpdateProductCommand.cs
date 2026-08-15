using Damoor.Application.Features.Products.Queries.GetProductById;
using MediatR;

namespace Damoor.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    int Id,
    string Name,
    string Description,
    int CategoryId) : IRequest<GetProductByIdResult>;
