using Damoor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class ProductImageConfiguration
    : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(2048);
        builder.HasOne(x => x.Product)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasIndex(
                x => x.ProductId,
                "IX_ProductImages_ProductId_Main")
            .IsUnique()
            .HasFilter("[IsMain] = 1");
        builder.HasIndex(
                x => x.ProductId,
                "IX_ProductImages_ProductId_Active")
            .IsUnique(false);
        builder.HasQueryFilter(x => !x.Product.IsDeleted);
    }
}
