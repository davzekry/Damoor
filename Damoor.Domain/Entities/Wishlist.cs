using Damoor.Domain.Common;

namespace Damoor.Domain.Entities;

public sealed class Wishlist : BaseEntity
{
    public int UserId { get; set; }
    public ICollection<WishlistItem> Items { get; set; } = [];
}
