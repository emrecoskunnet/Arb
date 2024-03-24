using Ardalis.GuardClauses;
using ArbTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceProductMerchant : InsertOnlyBaseEntity
{
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceProductMerchant()
    {
    }
    public MarketPlaceProductMerchant(int marketPlaceProductSaleId, int marketPlaceSellerId, bool buyBox, decimal price, int? stock, int? sellerTotalSaleCount)
    {
        Guard.Against.NegativeOrZero(marketPlaceProductSaleId);
        Guard.Against.NegativeOrZero(marketPlaceSellerId);
        Guard.Against.NegativeOrZero(price);
        
        MarketPlaceProductSaleId = marketPlaceProductSaleId;
        MarketPlaceSellerId = marketPlaceSellerId;
        BuyBox = buyBox;
        Price = price;
        Stock = stock;
        SellerTotalSaleCount = sellerTotalSaleCount;
    }
    public int MarketPlaceProductSaleId { get; private set; }
    public int MarketPlaceSellerId{ get; private set; }
    
    public bool BuyBox{ get; private set; }
    public decimal Price { get; private set; }
    public int? Stock { get; private set; }
    
    public int? SellerTotalSaleCount { get; private set; }
    
}
