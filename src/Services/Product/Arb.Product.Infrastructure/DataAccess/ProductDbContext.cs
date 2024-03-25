using System.Reflection;
using Arb.Product.Core;
using ArbTech.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Arb.Product.Infrastructure.DataAccess;

 
public class ProductDbContext : ArbTechDbContext
{
#pragma warning disable CS8618
    public ProductDbContext(DbContextOptions<ProductDbContext> options)
#pragma warning restore CS8618
        : base(options)
    {
    }

    public DbSet<MarketPlace> MarketPlaces { get; set; }
    public DbSet<MarketPlaceCategory> MarketPlaceCategories { get; set; }
    public DbSet<MarketPlaceMerchant> MarketPlaceMerchants { get; set; }
    public DbSet<MarketPlaceProduct> MarketPlaceProducts { get; set; }
    public DbSet<MarketPlaceProductSaleMerchant> MarketPlaceProductMerchants { get; set; }
    public DbSet<MarketPlaceProductSale> MarketPlaceProductSales { get; set; }
    public DbSet<Product.Core.Product> Products { get; set; }

    protected override void ApplyConfigurationOnModelCreating(ModelBuilder builder)
    { 
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); 
    }
}

// ReSharper disable once UnusedType.Global
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
{
    public ProductDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=arbdb;Username=postgres;Password=S@hin01.*;Enlist=true");

        return new ProductDbContext(optionsBuilder.Options);
    }
}

