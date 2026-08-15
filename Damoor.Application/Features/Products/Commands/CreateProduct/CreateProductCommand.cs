using Damoor.Application.Features.Products.Queries.GetProductById;
using MediatR;

namespace Damoor.Application.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    int CategoryId) : IRequest<GetProductByIdResult>;
