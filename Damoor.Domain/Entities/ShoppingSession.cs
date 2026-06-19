using Damoor.Domain.Common;

namespace Damoor.Domain.Entities;

public sealed class ShoppingSession : BaseEntity
{
    public string SessionToken { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Cart Cart { get; set; } = null!;
}
