using Ardalis.GuardClauses;
using ArbTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceProductSaleMerchant : InsertOnlyBaseEntity
{
    
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceProductSaleMerchant()
    {
    }

    public MarketPlaceProductSaleMerchant(int marketPlaceId, int marketPlaceProductSaleId, int marketPlaceMerchantId,
        bool buyBox, decimal price, int? stock, int? sellerTotalSaleCount)
    {
        Guard.Against.NegativeOrZero(marketPlaceId);
        Guard.Against.NegativeOrZero(marketPlaceProductSaleId);
        Guard.Against.NegativeOrZero(marketPlaceMerchantId);
        Guard.Against.NegativeOrZero(price);

        MarketPlaceId = marketPlaceId;
        MarketPlaceProductSaleId = marketPlaceProductSaleId;
        MarketPlaceMerchantId = marketPlaceMerchantId;
        BuyBox = buyBox;
        Price = price;
        Stock = stock;
        MerchantTotalSaleCount = sellerTotalSaleCount;
    }

    public int MarketPlaceProductSaleMerchantId { get; set; }
    public int MarketPlaceId { get; private set; }
    public int MarketPlaceProductSaleId { get; private set; }
    public int MarketPlaceMerchantId { get; private set; }

    public bool BuyBox { get; private set; }
    public decimal Price { get; private set; }
    public int? Stock { get; private set; }

    public int? MerchantTotalSaleCount { get; private set; }
}
