namespace Damoor.Application.Features.Products.Models;

public sealed class ProductImageModel
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}
