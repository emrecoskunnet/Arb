using System.Runtime.CompilerServices;
using Ardalis.SmartEnum;

namespace Arb.SharedKernel.Enums;

//   \\      potential      // => kvk onayı
//    \\     party    // 
//           XXX    ==>> kayıt olma
//    //    customer  \\ => satış
// public enum CustomerType { Lead = 8, Party = 9, Customer = 10 }

public class CustomerType : SmartEnum<CustomerType>
{
    public static readonly CustomerType All = new(0);
    public static readonly CustomerType Potential = new(1);
    public static readonly CustomerType Party = new(2);
    public static readonly CustomerType Customer = new(3);

    public CustomerType(short value, [CallerMemberName] string? name = null) : base(name, value)
    {
    }
}
