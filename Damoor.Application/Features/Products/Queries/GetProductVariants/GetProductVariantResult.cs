namespace Damoor.Application.Features.Products.Queries.GetProductVariants;

public sealed class GetProductVariantResult
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsInStock => StockQuantity > 0;
}
