using Damoor.Application.Common.Exceptions;
using Damoor.Application.Features.Orders.Models;
using Damoor.Domain.Entities;
using Damoor.Infrastructure.Identity;

namespace Damoor.Application.Features.Orders.Common;

internal static class OrderAccessor
{
    public static void EnsureAccessible(
        Order order,
        int? userId,
        string? sessionToken)
    {
        if (order.UserId.HasValue)
        {
            if (order.UserId != userId)
                throw new UnauthorizedException("You are not authorized to access this order.");

            return;
        }

        if (string.IsNullOrWhiteSpace(sessionToken) || order.SessionToken != sessionToken)
            throw new UnauthorizedException("You are not authorized to access this order.");
    }

    public static OrderDetailsResult ToDetailsResult(Order order)
        => new()
        {
            Id = order.Id,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            ShippingAddress = order.ShippingAddress,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(ToItemResult).ToList()
        };

    public static OrderItemResult ToItemResult(OrderItem item)
        => new()
        {
            Id = item.Id,
            ProductVariantId = item.ProductVariantId,
            ProductName = item.ProductName,
            VariantDescription = item.VariantDescription,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        };

    public static void RestockItems(Order order)
    {
        foreach (var item in order.Items.Where(x => x.ProductVariant is not null))
            item.ProductVariant!.StockQuantity += item.Quantity;
    }

    public static AdminOrderDetailsResult ToAdminDetailsResult(Order order, AppUser? user)
        => new()
        {
            Id = order.Id,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            ShippingAddress = order.ShippingAddress,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            UserId = order.UserId,
            CustomerName = user?.FullName,
            CustomerEmail = user?.Email,
            SessionToken = order.SessionToken,
            Items = order.Items.Select(ToItemResult).ToList()
        };
}
