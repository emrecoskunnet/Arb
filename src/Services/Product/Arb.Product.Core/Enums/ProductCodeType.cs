using System.Runtime.CompilerServices;
using Ardalis.SmartEnum;

namespace Arb.Product.Core.Enums;

public class ProductCodeType : SmartEnum<ProductCodeType>
{
    public static readonly ProductCodeType All = new(0);
    public static readonly ProductCodeType Barcode = new(1); 

    public ProductCodeType(short value, [CallerMemberName] string? name = null) : base(name, value)
    {
    }
}
