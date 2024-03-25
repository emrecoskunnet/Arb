using Ardalis.GuardClauses;
using ArbTech.SharedKernel;
using ArbTech.SharedKernel.Extensions;

namespace Arb.Product.Core;

public class MarketPlace : BaseEntity
{
    private readonly List<MarketPlaceCategory> _marketPlaceCategories = new();
    private readonly List<MarketPlaceMerchant> _marketPlaceMerchants = new();
    private readonly List<MarketPlaceProduct> _marketPlaceProducts = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable once UnusedMember.Local
    private MarketPlace()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public MarketPlace(string marketPlaceName)
    {
        Guard.Against.NullOrWhiteSpace(marketPlaceName);

        MarketPlaceName = marketPlaceName;
    }

    public int MarketPlaceId { get; set; }

    public string MarketPlaceName { get; private set; }

    public IReadOnlyCollection<MarketPlaceCategory> MarketPlaceCategories => _marketPlaceCategories.AsReadOnly();
    public IReadOnlyCollection<MarketPlaceMerchant> MarketPlaceMerchants => _marketPlaceMerchants.AsReadOnly();
    public IReadOnlyCollection<MarketPlaceProduct> MarketPlaceProducts => _marketPlaceProducts.AsReadOnly();

    public void AddCategory(int marketPlaceCategoryId, string categoryName, bool leaf, int? parentCategoryId,
        int? mainCategoryId,
        string? hierarchyPath = null)
    {
        Guard.Against.NegativeOrZero(marketPlaceCategoryId);
        Guard.Against.NullOrWhiteSpace(categoryName);
        MarketPlaceCategory? exists =
            _marketPlaceCategories.Find(i => i.MarketPlaceCategoryId == marketPlaceCategoryId);
        Guard.Against.DomainObjectAlreadyExists(categoryName, exists);

        _marketPlaceCategories.Add(new MarketPlaceCategory(MarketPlaceId, marketPlaceCategoryId, categoryName, leaf,
            parentCategoryId, mainCategoryId, hierarchyPath));
    }

    public void RemoveCategory(int marketPlaceCategoryId)
    {
        Guard.Against.NegativeOrZero(marketPlaceCategoryId);
        MarketPlaceCategory? exists =
            _marketPlaceCategories.Find(i => i.MarketPlaceCategoryId == marketPlaceCategoryId);
        Guard.Against.DomainObjectNotFound(marketPlaceCategoryId.ToString(), exists);

        exists.SoftDelete();
    }

    public void AddMerchant(int marketPlaceMerchantId, string merchantName, string? frontStoreLink = null)
    {
        Guard.Against.NegativeOrZero(marketPlaceMerchantId);
        Guard.Against.NullOrWhiteSpace(merchantName);

        MarketPlaceMerchant? exists = _marketPlaceMerchants.Find(i => i.MarketPlaceMerchantId == marketPlaceMerchantId);
        Guard.Against.DomainObjectAlreadyExists(marketPlaceMerchantId.ToString(), exists);

        _marketPlaceMerchants.Add(new MarketPlaceMerchant(MarketPlaceId, marketPlaceMerchantId, merchantName, frontStoreLink));
    }

    public void RemoveMerchant(int marketPlaceMerchantId)
    {
        Guard.Against.NegativeOrZero(marketPlaceMerchantId);

        MarketPlaceMerchant? exists = _marketPlaceMerchants.Find(i => i.MarketPlaceMerchantId == marketPlaceMerchantId);
        Guard.Against.DomainObjectNotFound(marketPlaceMerchantId.ToString(), exists);

        exists.SoftDelete();
    }

    public void AddProduct(int marketPlaceProductId, int productId, string productName, string productLink,
        string productImageLink, int marketPlaceCategoryId)
    {
        Guard.Against.NegativeOrZero(marketPlaceProductId);
        Guard.Against.NullOrWhiteSpace(productName);

        MarketPlaceProduct? exists = _marketPlaceProducts.Find(i => i.MarketPlaceProductId == marketPlaceProductId);
        Guard.Against.DomainObjectAlreadyExists(marketPlaceProductId.ToString(), exists);

        _marketPlaceProducts.Add(new MarketPlaceProduct(MarketPlaceId, marketPlaceProductId, productId, productName, productLink,
            productImageLink, marketPlaceCategoryId));
    }

    public void RemoveProduct(int marketPlaceProductId)
    {
        Guard.Against.NegativeOrZero(marketPlaceProductId);

        MarketPlaceProduct? exists = _marketPlaceProducts.Find(i => i.MarketPlaceProductId == marketPlaceProductId);
        Guard.Against.DomainObjectNotFound(marketPlaceProductId.ToString(), exists);

        exists.SoftDelete();
    }

    public MarketPlaceProductSale AddProductSale(int marketPlaceProductId, int categorySalesRank,
        int mainCategorySalesRank, decimal buyBox)
    {
        Guard.Against.NegativeOrZero(marketPlaceProductId);

        MarketPlaceProduct? exists =
            _marketPlaceProducts.Find(i =>
                i.MarketPlaceProductId == marketPlaceProductId && i.MarketPlaceId == MarketPlaceId);
        Guard.Against.DomainObjectNotFound(marketPlaceProductId.ToString(), exists);

        return exists.AddSale(categorySalesRank, mainCategorySalesRank, buyBox);
    }

    public void UpdateProductSalesCount(int marketPlaceProductSaleId, int salesCount)
    {
        Guard.Against.NegativeOrZero(marketPlaceProductSaleId);

        MarketPlaceProductSale? exists =
            _marketPlaceProducts.SelectMany(i => i.MarketPlaceProductSales)
                .FirstOrDefault(i => i.MarketPlaceProductSaleId == marketPlaceProductSaleId);
        Guard.Against.DomainObjectNotFound(marketPlaceProductSaleId.ToString(), exists);

        exists.UpdatePeriodSalesCount(salesCount);
    }

    public void AddProductSaleMerchant(int marketPlaceProductSaleId, int marketPlaceMerchantId,
        bool buyBox, decimal price, int? stock, int? sellerTotalSaleCount)
    {
        Guard.Against.NegativeOrZero(marketPlaceProductSaleId);
        Guard.Against.NegativeOrZero(marketPlaceMerchantId);

        MarketPlaceProductSale? exists =
            _marketPlaceProducts.SelectMany(i => i.MarketPlaceProductSales)
                .FirstOrDefault(i => i.MarketPlaceProductSaleId == marketPlaceProductSaleId);
        Guard.Against.DomainObjectNotFound(marketPlaceProductSaleId.ToString(), exists);

        exists.AddMerchant(marketPlaceMerchantId, buyBox, price, stock, sellerTotalSaleCount);
    }
}
