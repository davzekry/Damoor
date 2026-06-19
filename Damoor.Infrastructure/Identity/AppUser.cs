using Damoor.Domain.Common;
using Damoor.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Damoor.Infrastructure.Identity;

public class AppUser : IdentityUser<int>, ISoftDeletable
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<ShoppingSession> ShoppingSessions { get; set; } = [];
    public Wishlist? Wishlist { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
