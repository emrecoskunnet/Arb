using Arb.Product.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb.Product.Infrastructure.DataAccess.Configurations;

public class MarketPlaceMerchantConfiguration : IEntityTypeConfiguration<MarketPlaceMerchant>
{
    public void Configure(EntityTypeBuilder<MarketPlaceMerchant> builder)
    {
        builder.ToTable("MarketPlaceMerchants", "Product");
        builder.HasKey(i => new { i.MarketPlaceId, i.MarketPlaceMerchantId });
        
        builder.Property(i => i.MerchantName).HasMaxLength(1024);
        builder.Property(i => i.FrontStoreLink).HasMaxLength(2048);
    }
}
