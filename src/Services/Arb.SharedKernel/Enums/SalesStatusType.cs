using System.Runtime.CompilerServices;
using Ardalis.SmartEnum;

namespace Arb.SharedKernel.Enums;

//
//   ||        lost     ||
//   
//   \\  cold || return //
//    \\     warn      //
//     \\     hot     //
//            XXX    ==>> Satış
//      //   a | r   \\    active | reclaim
//     //     gain    \\
//
//

/// <summary>
///     7 gün geçince satış öncesi üste satış sonrası alta taşır
/// </summary>
//public enum CustomerSalesStatusType
// {
//    /// <summary>
//    /// Default, ilk defa geldi
//    /// </summary>
//    Cold = 11,

//    /// <summary>
//    /// satın almayan kişi geri geldi: 
//    /// </summary>
//    Return = 12,

//    /// <summary>
//    /// iletişim kuruldu  
//    /// </summary>
//    Warn = 13,

//    /// <summary>
//    /// police teklifi üretildi
//    /// </summary>
//    Hot = 14,

//    /// <summary>
//    /// satış  gerçekleşti -  poliçe kesildi
//    /// </summary>
//    Active = 15,

//    /// <summary>
//    /// tekrar satış gerçekleşti
//    /// </summary>
//    Reclaim = 16,

//    /// <summary>
//    /// sigorta bitti ve satış henüz olmadı
//    /// </summary>
//    Gain = 17,

//    /// <summary>
//    /// colda 14 gün geçti
//    /// </summary>
//    Lost = 18
//}
public class SalesStatusType : SmartEnum<SalesStatusType>
{
    public static readonly SalesStatusType All = new(0);
    public static readonly SalesStatusType Cold = new(4);
    public static readonly SalesStatusType Return = new(5);
    public static readonly SalesStatusType Warn = new(6);
    public static readonly SalesStatusType Hot = new(7);
    public static readonly SalesStatusType Active = new(8);
    public static readonly SalesStatusType Reclaim = new(9);
    public static readonly SalesStatusType Gain = new(10);
    public static readonly SalesStatusType Lost = new(11);

    protected SalesStatusType(short value, [CallerMemberName] string? name = null) : base(name, value) { }
}
