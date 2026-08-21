using Damoor.Domain.Common;

namespace Damoor.Domain.Entities;

public sealed class WishlistItem : BaseEntity
{
    public int WishlistId { get; set; }
    public Wishlist Wishlist { get; set; } = null!;
    public int ProductVariantId { get; set; }
    public ProductVariant Variant { get; set; } = null!;
}
