namespace Damoor.Application.Features.Wishlists.Models;

public sealed class WishlistItemResult
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? MainImageUrl { get; set; }
    public decimal? MinPrice { get; set; }
    public DateTime AddedAt { get; set; }
}
