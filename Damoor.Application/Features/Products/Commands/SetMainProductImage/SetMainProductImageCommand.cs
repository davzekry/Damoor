using Damoor.Application.Features.Products.Models;
using MediatR;

namespace Damoor.Application.Features.Products.Commands.SetMainProductImage;

public sealed record SetMainProductImageCommand(int Id)
    : IRequest<ProductImageModel>;
