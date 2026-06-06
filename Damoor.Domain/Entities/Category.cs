using Damoor.Domain.Common;

namespace Damoor.Domain.Entities
{
    public sealed class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = [];
    }
}
