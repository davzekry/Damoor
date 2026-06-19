using Damoor.Domain.Common;

namespace Damoor.Domain.Entities;

public sealed class Cart : BaseEntity
{
    public int ShoppingSessionId { get; set; }
    public ShoppingSession ShoppingSession { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = [];
}
