using System.Runtime.CompilerServices;
using Ardalis.SmartEnum;

namespace Arb.SharedKernel.Enums;
public class GenderType : SmartEnum<GenderType>
{
    public static readonly GenderType All = new(0);
    public static readonly GenderType Male = new(1);
    public static readonly GenderType Female = new(2);

    protected GenderType(short value, [CallerMemberName] string? name = null) : base(name, value) { }
}
