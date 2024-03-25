using Arb.Product.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb.Product.Infrastructure.DataAccess.Configurations;

public class MarketPlaceProductSaleConfiguration : IEntityTypeConfiguration<MarketPlaceProductSale>
{
    public void Configure(EntityTypeBuilder<MarketPlaceProductSale> builder)
    {
        builder.ToTable("MarketPlaceProductSales", "Product");

        builder.HasKey(i => i.MarketPlaceProductSaleId);
        
        builder.Metadata.FindNavigation(nameof(Core.MarketPlaceProductSale.MarketPlaceProductMerchants))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        
        
    }
}
