using Damoor.Domain.Entities;
using Damoor.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class WishlistConfiguration
    : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("Wishlists");
        builder.HasKey(x => x.Id);
        builder.HasOne<AppUser>()
            .WithOne(x => x.Wishlist)
            .HasForeignKey<Wishlist>(x => x.UserId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasIndex(x => x.UserId)
            .IsUnique();
    }
}
