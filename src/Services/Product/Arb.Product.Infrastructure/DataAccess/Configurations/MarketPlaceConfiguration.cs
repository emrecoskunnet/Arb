using Arb.Product.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb.Product.Infrastructure.DataAccess.Configurations;

public class MarketPlaceConfiguration : IEntityTypeConfiguration<MarketPlace>
{
    public void Configure(EntityTypeBuilder<MarketPlace> builder)
    {
        builder.ToTable("MarketPlaces", "Product");

        builder.Property(i => i.MarketPlaceName).HasMaxLength(512);
        builder.HasKey(i => i.MarketPlaceId);
        
        builder.Metadata.FindNavigation(nameof(Core.MarketPlace.MarketPlaceCategories))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Metadata.FindNavigation(nameof(Core.MarketPlace.MarketPlaceProducts))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Metadata.FindNavigation(nameof(Core.MarketPlace.MarketPlaceMerchants))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
