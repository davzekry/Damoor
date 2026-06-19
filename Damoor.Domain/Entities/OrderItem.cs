using Damoor.Domain.Common;

namespace Damoor.Domain.Entities;

public sealed class OrderItem : BaseEntity, INonDeletable
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string VariantDescription { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
