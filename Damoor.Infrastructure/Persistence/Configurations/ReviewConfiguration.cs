using Damoor.Domain.Entities;
using Damoor.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class ReviewConfiguration
    : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable(
            "Reviews",
            table => table.HasCheckConstraint(
                "CK_Reviews_Rating_Range",
                "[Rating] BETWEEN 1 AND 5"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Comment)
            .HasMaxLength(2000);
        builder.HasOne(x => x.Product)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasOne<AppUser>()
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.UserId);
        builder.HasQueryFilter(x =>
            !x.IsDeleted &&
            !x.Product.IsDeleted);
    }
}
