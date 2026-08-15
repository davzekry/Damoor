using MediatR;

namespace Damoor.Application.Features.Products.Commands.DeleteProductImage;

public sealed record DeleteProductImageCommand(int Id) : IRequest;
