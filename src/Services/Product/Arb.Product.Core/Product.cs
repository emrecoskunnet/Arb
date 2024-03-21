using Arb.Product.Core.Enums;
using Ardalis.GuardClauses;
using TebaTech.SharedKernel;
using TebaTech.SharedKernel.Extensions;

namespace Arb.Product.Core;

public class Product: BaseEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable once UnusedMember.Local
    private Product()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        
    }
    public Product(ProductCodeType productCodeType, string productCode)
    {
        Guard.Against.InvalidState(ProductCodeType,
            ProductCodeType.List.Where(i => i != ProductCodeType.All).ToArray());
        Guard.Against.NullOrWhiteSpace(productCode);
        
        ProductCodeType = productCodeType;
        ProductCode = productCode;
    }
    
    public ProductCodeType ProductCodeType { get; private set; } 
    public string ProductCode { get; private set; }
}
