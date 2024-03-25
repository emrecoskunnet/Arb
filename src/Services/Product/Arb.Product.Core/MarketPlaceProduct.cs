using Ardalis.GuardClauses;
using ArbTech.SharedKernel;

namespace Arb.Product.Core;

public class MarketPlaceProduct : InsertOnlyBaseEntity
{
    private readonly List<MarketPlaceProductSale> _marketPlaceProductSales = new();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable once UnusedMember.Local
    private MarketPlaceProduct()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public MarketPlaceProduct(int marketPlaceId, int marketPlaceProductId, int productId, string productName,
        string productLink,
        string productImageLink, int marketPlaceCategoryId)
    {
        Guard.Against.NegativeOrZero(marketPlaceId);
        Guard.Against.NegativeOrZero(marketPlaceProductId);
        Guard.Against.NegativeOrZero(productId);
        Guard.Against.NullOrWhiteSpace(productName);
        Guard.Against.NullOrWhiteSpace(productLink);
        Guard.Against.NullOrWhiteSpace(productImageLink);
        Guard.Against.NegativeOrZero(marketPlaceCategoryId);

        MarketPlaceId = marketPlaceId;
        ProductId = productId;
        ProductName = productName;
        ProductLink = productLink;
        ProductImageLink = productImageLink;
        MarketPlaceCategoryId = marketPlaceCategoryId;
    }

    public int MarketPlaceId { get; private set; }
    public int MarketPlaceProductId { get; private set; }
    public int ProductId { get; private set; }

    public Product? Product { get; set; }

    public string ProductName { get; private set; }
    public string ProductLink { get; private set; }
    public string ProductImageLink { get; private set; }

    public int MarketPlaceCategoryId { get; private set; }

    public IReadOnlyCollection<MarketPlaceProductSale> MarketPlaceProductSales => _marketPlaceProductSales.AsReadOnly();

    public MarketPlaceProductSale AddSale(int categorySalesRank, int mainCategorySalesRank, decimal buyBox)
    {
        MarketPlaceProductSale? exists = null;
        switch (_marketPlaceProductSales.Count)
        {
            case > 1:
                _marketPlaceProductSales.ForEach(i => i.SoftDelete());
                break;
            case 1:
            {
                MarketPlaceProductSale first = _marketPlaceProductSales[0];
                if (first.CategorySalesRank == categorySalesRank && first.MainCategorySalesRank == mainCategorySalesRank
                                                                 && first.BuyBox == buyBox) exists = first;
            }
                break;
        }

        if (exists != null) return exists;
        MarketPlaceProductSale newSale = new(MarketPlaceId, MarketPlaceProductId, categorySalesRank,
            mainCategorySalesRank, buyBox);
        _marketPlaceProductSales.Add(newSale);
        return newSale;
    }
}
