using Ardalis.GuardClauses;
using TebaTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceCategory : BaseEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceCategory()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
    public MarketPlaceCategory(int marketPlaceId, string categoryName, int? parentCategoryId, int? mainCategoryId)
    {
        Guard.Against.NegativeOrZero(marketPlaceId);
        Guard.Against.NullOrWhiteSpace(categoryName);
        
        MarketPlaceId = marketPlaceId;
        CategoryName = categoryName;
        ParentCategoryId = parentCategoryId;
        MainCategoryId = mainCategoryId;
    }
    
    public int MarketPlaceId { get; private set; }
    public string CategoryName { get; private set; }
    public int? ParentCategoryId { get; private set; }
    public int? MainCategoryId { get; private set; }
}
