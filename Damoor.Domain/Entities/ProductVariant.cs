using Damoor.Domain.Common;

namespace Damoor.Domain.Entities;

public sealed class ProductVariant : SoftDeletableEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string SKU { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
