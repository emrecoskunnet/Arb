using Ardalis.GuardClauses;
using ArbTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceCategory : InsertOnlyBaseEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceCategory()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
    public MarketPlaceCategory(int marketPlaceId, int marketPlaceCategoryId, string categoryName, bool leaf, int? parentCategoryId, int? mainCategoryId,
        string? hierarchyPath=null)
    {
        Guard.Against.NegativeOrZero(marketPlaceId);
        Guard.Against.NegativeOrZero(marketPlaceCategoryId);
        Guard.Against.NullOrWhiteSpace(categoryName);
        
        MarketPlaceId = marketPlaceId;
        MarketPlaceCategoryId = marketPlaceCategoryId;
        CategoryName = categoryName;
        MarketPlaceParentCategoryId = parentCategoryId;
        MarketPlaceMainCategoryId = mainCategoryId;
        Leaf = leaf;
        HierarchyPath = hierarchyPath;
    }
    
    public int MarketPlaceId { get; private set; }
    public int MarketPlaceCategoryId { get; private set; }
    public string CategoryName { get; private set; }
    public int? MarketPlaceParentCategoryId { get; private set; }
    public int? MarketPlaceMainCategoryId { get; private set; }
    public bool Leaf { get; private set; }
    public string? HierarchyPath { get; private set; }
    
    public MarketPlaceCategory? ParentCategory { get; set; }
    
    public List<MarketPlaceCategory>? ChildrenCategories { get; set; }
}
