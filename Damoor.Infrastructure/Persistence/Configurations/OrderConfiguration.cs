using Damoor.Domain.Entities;
using Damoor.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration
    : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(
            "Orders",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_Orders_TotalAmount_NonNegative",
                    "[TotalAmount] >= 0");
                table.HasCheckConstraint(
                    "CK_Orders_CustomerIdentifier",
                    "([UserId] IS NOT NULL AND [SessionToken] IS NULL) OR " +
                    "([UserId] IS NULL AND [SessionToken] IS NOT NULL)");
                table.HasCheckConstraint(
                    "CK_Orders_Status",
                    "[Status] IN ('Pending', 'Confirmed', 'Processing', " +
                    "'Shipped', 'Delivered', 'Cancelled')");
            });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SessionToken)
            .HasMaxLength(128);
        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(x => x.ShippingAddress)
            .IsRequired()
            .HasMaxLength(1000);
        builder.Property(x => x.CustomerName)
            .IsRequired()
            .HasMaxLength(200)
            .HasDefaultValue(string.Empty);
        builder.Property(x => x.WhatsAppNumber)
            .IsRequired()
            .HasMaxLength(32)
            .HasDefaultValue(string.Empty);
        builder.Property(x => x.BackupPhoneNumber)
            .HasMaxLength(32);
        builder.Property(x => x.Notes)
            .HasMaxLength(1000);
        builder.HasOne<AppUser>()
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasIndex(x => new { x.UserId, x.CreatedAt })
            .HasFilter("[UserId] IS NOT NULL");
        builder.HasIndex(x => new { x.SessionToken, x.CreatedAt })
            .HasFilter("[SessionToken] IS NOT NULL");
    }
}
