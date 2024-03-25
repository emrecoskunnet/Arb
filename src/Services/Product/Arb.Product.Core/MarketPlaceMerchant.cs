using Ardalis.GuardClauses;
using ArbTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceMerchant : InsertOnlyBaseEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceMerchant()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
    public MarketPlaceMerchant(int marketPlaceId, int marketPlaceMerchantId, string merchantName, string? frontStoreLink = null)
    {
        Guard.Against.NegativeOrZero(marketPlaceId);
        Guard.Against.NullOrWhiteSpace(merchantName);
        Guard.Against.NegativeOrZero(marketPlaceMerchantId);
            
        MarketPlaceId = marketPlaceId;
        MarketPlaceMerchantId = marketPlaceMerchantId;
        MerchantName = merchantName;
        FrontStoreLink = frontStoreLink;
    }
    
    public int MarketPlaceId { get; private set; }
    public int MarketPlaceMerchantId { get; private set; }
    public string MerchantName { get; private set; }
    public string? FrontStoreLink{ get; private set; }
    
}

