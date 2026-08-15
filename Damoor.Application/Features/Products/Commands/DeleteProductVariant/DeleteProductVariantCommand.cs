using MediatR;

namespace Damoor.Application.Features.Products.Commands.DeleteProductVariant;

public sealed record DeleteProductVariantCommand(int Id) : IRequest;
