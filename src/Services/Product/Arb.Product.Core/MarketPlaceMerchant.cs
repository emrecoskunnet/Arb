using Ardalis.GuardClauses;
using ArbTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceMerchant : BaseEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceMerchant()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
    public MarketPlaceMerchant(int marketPlaceId, string sellerName, string? frontStoreLink)
    {
        Guard.Against.NegativeOrZero(marketPlaceId);
        Guard.Against.NullOrWhiteSpace(sellerName);
            
        MarketPlaceId = marketPlaceId;
        MerchantName = sellerName;
        FrontStoreLink = frontStoreLink;
    }
    
    public int MarketPlaceId { get; private set; }
    public string MerchantName { get; private set; }
    public string? FrontStoreLink{ get; private set; }
    
}

