using Damoor.Application.Features.Products.Models;
using MediatR;

namespace Damoor.Application.Features.Products.Commands.UpdateProductVariant;

public sealed record UpdateProductVariantCommand(
    int Id,
    string SKU,
    string Size,
    string Color,
    decimal Price,
    decimal? SalePrice,
    int StockQuantity) : IRequest<ProductVariantModel>;
