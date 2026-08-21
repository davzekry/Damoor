namespace Damoor.Application.Features.Wishlists.Models;

public sealed class WishlistItemResult
{
    public int Id { get; set; }
    public int ProductVariantId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public string? MainImageUrl { get; set; }
    public DateTime AddedAt { get; set; }
}
