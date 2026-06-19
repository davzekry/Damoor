using Damoor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class CartItemConfiguration
    : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable(
            "CartItems",
            table => table.HasCheckConstraint(
                "CK_CartItems_Quantity_Positive",
                "[Quantity] > 0"));
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Cart)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ProductVariant)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasIndex(x => new { x.CartId, x.ProductVariantId })
            .IsUnique();
        builder.HasQueryFilter(x =>
            !x.ProductVariant.IsDeleted &&
            !x.ProductVariant.Product.IsDeleted);
    }
}
