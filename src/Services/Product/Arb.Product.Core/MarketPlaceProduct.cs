using Ardalis.GuardClauses;
using TebaTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceProduct: BaseEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceProduct()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
    public MarketPlaceProduct(int marketPlaceId, int productId, string productName, string productLink, string productImageLink, int marketPlaceCategoryId, int marketPlaceMainCategoryId)
    {
        Guard.Against.NegativeOrZero(marketPlaceId);
        Guard.Against.NegativeOrZero(productId);
        Guard.Against.NullOrWhiteSpace(productName);
        Guard.Against.NullOrWhiteSpace(productLink);
        Guard.Against.NullOrWhiteSpace(productImageLink);
        Guard.Against.NegativeOrZero(marketPlaceMainCategoryId);
        Guard.Against.NegativeOrZero(marketPlaceCategoryId);
        
        MarketPlaceId = marketPlaceId;
        ProductId = productId;
        ProductName = productName;
        ProductLink = productLink;
        ProductImageLink = productImageLink;
        MarketPlaceCategoryId = marketPlaceCategoryId;
        MarketPlaceMainCategoryId = marketPlaceMainCategoryId;
    }
    public int MarketPlaceId { get; private set; }
    public int ProductId { get; private set; }
    
    public string ProductName { get; private set; }
    public string ProductLink { get; private set; }
    public string ProductImageLink { get; private set; }
    
    public int MarketPlaceCategoryId { get; private set; }
    public int MarketPlaceMainCategoryId { get; private set; }
}
