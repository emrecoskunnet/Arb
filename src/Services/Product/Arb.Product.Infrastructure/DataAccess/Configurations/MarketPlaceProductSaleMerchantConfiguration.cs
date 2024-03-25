using Arb.Product.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb.Product.Infrastructure.DataAccess.Configurations;

public class MarketPlaceProductSaleMerchantConfiguration : IEntityTypeConfiguration<MarketPlaceProductSaleMerchant>
{
    public void Configure(EntityTypeBuilder<MarketPlaceProductSaleMerchant> builder)
    {
        builder.ToTable("MarketPlaceProductSaleMerchants", "Product");

        builder.HasKey(i => i.MarketPlaceProductSaleMerchantId);

        builder.HasOne<MarketPlaceMerchant>()
            .WithMany()
            .HasForeignKey(i => new { i.MarketPlaceId, i.MarketPlaceMerchantId });
    }
}
