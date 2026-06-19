using Damoor.Domain.Entities;
using Damoor.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class ShoppingSessionConfiguration
    : IEntityTypeConfiguration<ShoppingSession>
{
    public void Configure(EntityTypeBuilder<ShoppingSession> builder)
    {
        builder.ToTable("ShoppingSessions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SessionToken)
            .IsRequired()
            .HasMaxLength(128);
        builder.Property(x => x.ExpiresAt)
            .IsRequired();
        builder.HasOne<AppUser>()
            .WithMany(x => x.ShoppingSessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasIndex(x => x.SessionToken)
            .IsUnique();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ExpiresAt);
    }
}
