using Ardalis.GuardClauses;
using TebaTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceSeller : BaseEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceSeller()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
    public MarketPlaceSeller(int marketPlaceId, string sellerName, string? frontStoreLink)
    {
        Guard.Against.NegativeOrZero(marketPlaceId);
        Guard.Against.NullOrWhiteSpace(sellerName);
            
        MarketPlaceId = marketPlaceId;
        SellerName = sellerName;
        FrontStoreLink = frontStoreLink;
    }
    
    public int MarketPlaceId { get; private set; }
    public string SellerName { get; private set; }
    public string? FrontStoreLink{ get; private set; }
    
}

