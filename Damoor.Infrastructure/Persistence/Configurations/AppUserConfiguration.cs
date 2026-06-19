using Damoor.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class AppUserConfiguration
    : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.HasIndex(x => x.NormalizedUserName)
            .HasDatabaseName("UserNameIndex")
            .IsUnique()
            .HasFilter(
                "[NormalizedUserName] IS NOT NULL AND [IsDeleted] = 0");
        builder.HasIndex(x => x.NormalizedEmail)
            .HasDatabaseName("EmailIndex")
            .IsUnique()
            .HasFilter(
                "[NormalizedEmail] IS NOT NULL AND [IsDeleted] = 0");
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
