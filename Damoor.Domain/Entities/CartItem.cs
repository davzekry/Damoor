using Damoor.Domain.Common;

namespace Damoor.Domain.Entities;

public sealed class CartItem : BaseEntity
{
    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public int Quantity { get; set; }
}
