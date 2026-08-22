using Damoor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class WishlistItemConfiguration
    : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder.ToTable("WishlistItems");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Wishlist)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ProductVariant)
            .WithMany(x => x.WishlistItems)
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasIndex(x => new { x.WishlistId, x.ProductVariantId })
            .IsUnique();
        builder.HasQueryFilter(x =>
            !x.ProductVariant.IsDeleted &&
            !x.ProductVariant.Product.IsDeleted);
    }
}
