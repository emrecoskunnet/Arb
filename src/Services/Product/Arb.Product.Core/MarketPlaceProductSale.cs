using Ardalis.GuardClauses;
using ArbTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceProductSale : InsertOnlyBaseEntity
{
    private List<MarketPlaceProductSaleMerchant> _marketPlaceProductMerchants = new();
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceProductSale()
    {
    }

    public MarketPlaceProductSale(int marketPlaceId, int marketPlaceProductId, int categorySalesRank,
        int mainCategorySalesRank, decimal buyBox)
    {
        Guard.Against.NegativeOrZero(marketPlaceId);
        Guard.Against.NegativeOrZero(marketPlaceProductId);
        Guard.Against.NegativeOrZero(categorySalesRank);
        Guard.Against.NegativeOrZero(mainCategorySalesRank);
        Guard.Against.NegativeOrZero(buyBox);

        MarketPlaceId = marketPlaceId;
        MarketPlaceProductId = marketPlaceProductId;
        CategorySalesRank = categorySalesRank;
        MainCategorySalesRank = mainCategorySalesRank;
        BuyBox = buyBox;
    }

    public int MarketPlaceProductSaleId { get; set; }

    public int MarketPlaceId { get; private set; } 
    public int MarketPlaceProductId { get; private set; }

    public int CategorySalesRank { get; private set; }
    public int MainCategorySalesRank { get; private set; }

    public decimal BuyBox { get; private set; }

    public int? PeriodSalesCount { get; private set; }

    internal void UpdatePeriodSalesCount(int salesCount)
    {
        PeriodSalesCount = salesCount;
    }
    
    public IReadOnlyCollection<MarketPlaceProductSaleMerchant> MarketPlaceProductMerchants =>
        _marketPlaceProductMerchants.AsReadOnly();

    internal void AddMerchant(int marketPlaceMerchantId, bool buyBox, decimal price, int? stock,
        int? sellerTotalSaleCount)
    {
        Guard.Against.NegativeOrZero(marketPlaceMerchantId);
        MarketPlaceProductSaleMerchant? exists = _marketPlaceProductMerchants.Find(i =>
            i.MarketPlaceMerchantId == marketPlaceMerchantId && i.MarketPlaceId == MarketPlaceId);
        exists?.SoftDelete();

        _marketPlaceProductMerchants.Add(new MarketPlaceProductSaleMerchant(MarketPlaceId, MarketPlaceProductSaleId,
            marketPlaceMerchantId, buyBox, price, stock, sellerTotalSaleCount));
    }
}
