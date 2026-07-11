using Damoor.Application.Features.Products.Models;

namespace Damoor.Application.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal? MinPrice { get; set; }
    public int TotalStockQuantity { get; set; }
    public double? AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<ProductImageModel> Images { get; set; } = [];
    public List<ProductVariantModel> Variants { get; set; } = [];
}
