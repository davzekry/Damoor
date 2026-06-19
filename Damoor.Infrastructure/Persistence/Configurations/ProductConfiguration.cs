using Damoor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Damoor.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration
    : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);
        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasIndex(x => new { x.Name, x.Id })
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.CreatedAt, x.Id })
            .HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(x =>
            !x.IsDeleted &&
            !x.Category.IsDeleted);
    }
}
