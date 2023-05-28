using Commerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commerce.Infrastructure.Database.TablesConfigurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(120);
        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(5,2);
        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(p => p.SubCategory)
            .HasMaxLength(20);
        builder.Property(p => p.Manufacturer)
            .HasMaxLength(20);
    }
}