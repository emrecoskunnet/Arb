using Ardalis.GuardClauses;
using ArbTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceProductMerchant : InsertOnlyBaseEntity
{
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceProductMerchant()
    {
    }
    public MarketPlaceProductMerchant(int marketPlaceProductSaleId, int marketPlaceMerchantId, bool buyBox, decimal price, int? stock, int? sellerTotalSaleCount)
    {
        Guard.Against.NegativeOrZero(marketPlaceProductSaleId);
        Guard.Against.NegativeOrZero(marketPlaceMerchantId);
        Guard.Against.NegativeOrZero(price);
        
        MarketPlaceProductSaleId = marketPlaceProductSaleId;
        MarketPlaceMerchantId = marketPlaceMerchantId;
        BuyBox = buyBox;
        Price = price;
        Stock = stock;
        MerchantTotalSaleCount = sellerTotalSaleCount;
    }
    public int MarketPlaceProductSaleId { get; private set; }
    public int MarketPlaceMerchantId{ get; private set; }
    
    public bool BuyBox{ get; private set; }
    public decimal Price { get; private set; }
    public int? Stock { get; private set; }
    
    public int? MerchantTotalSaleCount { get; private set; }
    
}
