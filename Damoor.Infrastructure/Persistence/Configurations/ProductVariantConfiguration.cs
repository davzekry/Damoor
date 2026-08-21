using Damoor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class ProductVariantConfiguration
    : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable(
            "ProductVariants",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_ProductVariants_Price_NonNegative",
                    "[Price] >= 0");
                table.HasCheckConstraint(
                    "CK_ProductVariants_StockQuantity_NonNegative",
                    "[StockQuantity] >= 0");
                table.HasCheckConstraint(
                    "CK_ProductVariants_SalePrice_Valid",
                    "[SalePrice] IS NULL OR ([SalePrice] >= 0 AND [SalePrice] <= [Price])");
            });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SKU)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(x => x.Size)
            .IsRequired()
            .HasMaxLength(32);
        builder.Property(x => x.Color)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(x => x.Price)
            .HasPrecision(18, 2)
            .IsRequired();
        builder.Property(x => x.SalePrice)
            .HasPrecision(18, 2);
        builder.Property(x => x.StockQuantity)
            .HasDefaultValue(0);
        builder.HasOne(x => x.Product)
            .WithMany(x => x.Variants)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasIndex(x => x.SKU)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.ProductId, x.Size, x.Color })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(x =>
            !x.IsDeleted &&
            !x.Product.IsDeleted);
    }
}
