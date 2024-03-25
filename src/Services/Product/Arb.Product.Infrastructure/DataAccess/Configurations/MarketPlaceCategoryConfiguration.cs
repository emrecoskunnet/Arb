using Arb.Product.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arb.Product.Infrastructure.DataAccess.Configurations;

public class MarketPlaceCategoryConfiguration : IEntityTypeConfiguration<MarketPlaceCategory>
{
    public void Configure(EntityTypeBuilder<MarketPlaceCategory> builder)
    {
        builder.ToTable("MarketPlaceCategories", "Product");

        builder.HasKey(i => new { i.MarketPlaceId, i.MarketPlaceCategoryId });
        builder.Property(i => i.CategoryName).HasMaxLength(512);
        builder.Property(i => i.HierarchyPath).HasMaxLength(2048);


        builder.HasOne(i => i.ParentCategory)
            .WithMany(i => i.ChildrenCategories)
            .HasForeignKey(i => new { i.MarketPlaceId, i.MarketPlaceParentCategoryId})
            .HasPrincipalKey(i => new { i.MarketPlaceId, i.MarketPlaceCategoryId});
    }
}
