using Damoor.Domain.Common;

namespace Damoor.Domain.Entities
{
    public sealed class Category : SoftDeletableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<Product> Products { get; set; } = [];
    }
}
