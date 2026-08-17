namespace Damoor.Application.Features.Carts.Models;

public sealed class CartItemResult
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int ProductVariantId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
    public string? MainImageUrl { get; set; }
}

public sealed class CartResult
{
    public int CartId { get; set; }
    public List<CartItemResult> Items { get; set; } = [];
    public decimal Total => Items.Sum(x => x.LineTotal);
}
