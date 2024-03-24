using Ardalis.GuardClauses;
using ArbTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceProductSales : InsertOnlyBaseEntity
{
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceProductSales()
    {
    }
    public MarketPlaceProductSales(int marketPlaceProductId, int categorySalesRank, int mainCategorySalesRank, decimal buyBox, int? buyBoxStock, int? buyBoxProductSellerId, int? periodSalesCount)
    {
        Guard.Against.NegativeOrZero(marketPlaceProductId);
        Guard.Against.NegativeOrZero(categorySalesRank);
        Guard.Against.NegativeOrZero(mainCategorySalesRank);
        Guard.Against.NegativeOrZero(buyBox);
        
        MarketPlaceProductId = marketPlaceProductId;
        CategorySalesRank = categorySalesRank;
        MainCategorySalesRank = mainCategorySalesRank;
        BuyBox = buyBox;
        BuyBoxStock = buyBoxStock;
        BuyBoxProductSellerId = buyBoxProductSellerId;
        PeriodSalesCount = periodSalesCount;
    }
    public int MarketPlaceProductId { get; private set; }
    
    public int CategorySalesRank { get; private set; }
    public int MainCategorySalesRank { get; private set; }
    
    public decimal BuyBox { get; private set; }
    public int? BuyBoxStock { get; private set; }
    public int? BuyBoxProductSellerId  { get; private set; }
    
    public int? PeriodSalesCount{ get; private set; }
     
}
