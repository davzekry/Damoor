using Damoor.Domain.Entities.Enums;

namespace Damoor.Application.Features.Orders.Models;

public sealed class AdminOrderSummaryResult
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ItemCount { get; set; }
    public int? UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string WhatsAppNumber { get; set; } = string.Empty;
    public string? AccountEmail { get; set; }
    public string? SessionToken { get; set; }
}
