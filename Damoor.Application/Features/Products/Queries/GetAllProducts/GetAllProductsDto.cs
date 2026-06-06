namespace Damoor.Application.Features.Products.Queries.GetAllProducts;

public sealed class GetAllProductsDto
{
    public int Id {  get; set; }
    public string? Name { get; set; }
    public string? Description {  get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
    public string? CategoryName { get; set; }

};