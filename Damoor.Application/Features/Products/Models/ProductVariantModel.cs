namespace Damoor.Application.Features.Products.Models;

public sealed class ProductVariantModel
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsInStock => StockQuantity > 0;
    public List<ProductImageModel> Images { get; set; } = [];
}
