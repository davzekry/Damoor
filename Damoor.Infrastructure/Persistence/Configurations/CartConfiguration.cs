using Damoor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.ShoppingSession)
            .WithOne(x => x.Cart)
            .HasForeignKey<Cart>(x => x.ShoppingSessionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.ShoppingSessionId)
            .IsUnique();
    }
}
