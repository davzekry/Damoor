namespace Damoor.Application.Features.Products.Queries.GetAllProducts;

public sealed class GetAllProductsResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? MainImageUrl { get; set; }
    public decimal? MinPrice { get; set; }
    public int TotalStockQuantity { get; set; }
}
