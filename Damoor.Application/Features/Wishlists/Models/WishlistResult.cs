namespace Damoor.Application.Features.Wishlists.Models;

public sealed class WishlistResult
{
    public int WishlistId { get; set; }
    public List<WishlistItemResult> Items { get; set; } = [];
}
