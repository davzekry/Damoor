using Damoor.Domain.Entities.Enums;

namespace Damoor.Application.Features.Orders.Models;

public sealed class OrderDetailsResult
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string WhatsAppNumber { get; set; } = string.Empty;
    public string? BackupPhoneNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResult> Items { get; set; } = [];
}
