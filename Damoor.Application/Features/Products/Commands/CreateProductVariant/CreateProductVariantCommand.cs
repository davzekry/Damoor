using Damoor.Application.Features.Products.Models;
using MediatR;

namespace Damoor.Application.Features.Products.Commands.CreateProductVariant;

public sealed record CreateProductVariantCommand(
    int ProductId,
    string SKU,
    string Size,
    string Color,
    decimal Price,
    int StockQuantity) : IRequest<ProductVariantModel>;
