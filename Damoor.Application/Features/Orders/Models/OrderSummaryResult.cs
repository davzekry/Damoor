using Damoor.Domain.Entities.Enums;

namespace Damoor.Application.Features.Orders.Models;

public sealed class OrderSummaryResult
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ItemCount { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}
