using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb.Product.Infrastructure.DataAccess.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Core.Product>
{
    public void Configure(EntityTypeBuilder<Core.Product> builder)
    {
        builder.ToTable("Products", "Product");

        builder.HasKey(i => i.ProductId);

        builder.Property(i => i.ProductCode).HasMaxLength(256);
        
        builder.HasIndex(i => new { i.ProductCodeType, i.ProductCode }).IsUnique();
    }
}
