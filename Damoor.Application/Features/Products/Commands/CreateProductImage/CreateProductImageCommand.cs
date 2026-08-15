using Damoor.Application.Features.Products.Models;
using MediatR;

namespace Damoor.Application.Features.Products.Commands.CreateProductImage;

public sealed record CreateProductImageCommand(
    int ProductId,
    string ImageUrl,
    bool IsMain) : IRequest<ProductImageModel>;
