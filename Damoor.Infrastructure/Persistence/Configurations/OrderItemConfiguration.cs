using Damoor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration
    : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable(
            "OrderItems",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_OrderItems_Quantity_Positive",
                    "[Quantity] > 0");
                table.HasCheckConstraint(
                    "CK_OrderItems_UnitPrice_NonNegative",
                    "[UnitPrice] >= 0");
            });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProductName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.VariantDescription)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();
        builder.HasOne(x => x.Order)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasOne(x => x.ProductVariant)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.ProductVariantId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasIndex(x => x.OrderId);
    }
}
