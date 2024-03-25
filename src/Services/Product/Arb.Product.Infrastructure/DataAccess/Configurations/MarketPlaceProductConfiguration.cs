using Arb.Product.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb.Product.Infrastructure.DataAccess.Configurations;

public class MarketPlaceProductConfiguration : IEntityTypeConfiguration<MarketPlaceProduct>
{
    public void Configure(EntityTypeBuilder<MarketPlaceProduct> builder)
    {
        builder.ToTable("MarketPlaceProducts", "Product");

        builder.HasKey(i => new { i.MarketPlaceId, i.MarketPlaceProductId });
        builder.Property(i => i.ProductName).HasMaxLength(1024);
        builder.Property(i => i.ProductLink).HasMaxLength(1024);
        builder.Property(i => i.ProductImageLink).HasMaxLength(1024);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId);
        
        builder.Metadata.FindNavigation(nameof(Core.MarketPlaceProduct.MarketPlaceProductSales))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany<MarketPlaceProductSale>(nameof(Core.MarketPlaceProduct.MarketPlaceProductSales))
            .WithOne()
            .HasForeignKey(i => new { i.MarketPlaceId, i.MarketPlaceProductId });
    }
}
