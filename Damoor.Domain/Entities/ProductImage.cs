using Damoor.Domain.Common;

namespace Damoor.Domain.Entities;

public sealed class ProductImage : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int? ProductVariantId { get; set; }
    public ProductVariant? Variant { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}
