using Damoor.Application.Features.Products.Models;
using MediatR;

namespace Damoor.Application.Features.Products.Commands.CreateProductVariant;

public sealed record CreateProductVariantCommand(
    int ProductId,
    IReadOnlyList<CreateProductVariantItem> Variants)
    : IRequest<List<ProductVariantModel>>;

public sealed record CreateProductVariantItem(
    string SKU,
    string Size,
    string Color,
    decimal Price,
    decimal? SalePrice,
    int StockQuantity,
    IReadOnlyList<CreateProductVariantImageItem> Images);

public sealed record CreateProductVariantImageItem(
    string ImageUrl,
    bool IsMain);
