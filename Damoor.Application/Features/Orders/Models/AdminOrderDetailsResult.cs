using Damoor.Domain.Entities.Enums;

namespace Damoor.Application.Features.Orders.Models;

public sealed class AdminOrderDetailsResult
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UserId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? SessionToken { get; set; }
    public List<OrderItemResult> Items { get; set; } = [];
}
