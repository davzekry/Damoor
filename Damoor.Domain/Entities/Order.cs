using Damoor.Domain.Common;
using Damoor.Domain.Entities.Enums;

namespace Damoor.Domain.Entities;

public sealed class Order : BaseEntity, INonDeletable
{
    public int? UserId { get; set; }
    public string? SessionToken { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string ShippingAddress { get; set; } = string.Empty;
    public ICollection<OrderItem> Items { get; set; } = [];
}
