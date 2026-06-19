using Damoor.Domain.Common;

namespace Damoor.Domain.Entities
{
    public sealed class Product : SoftDeletableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<ProductVariant> Variants { get; set; } = [];
        public ICollection<ProductImage> Images { get; set; } = [];
        public ICollection<WishlistItem> WishlistItems { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
