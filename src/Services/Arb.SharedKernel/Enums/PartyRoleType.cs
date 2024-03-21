using System.Runtime.CompilerServices;
using Ardalis.SmartEnum;

namespace Arb.SharedKernel.Enums;

public class PartyRoleType : SmartEnum<PartyRoleType>
{
    public static readonly PartyRoleType User = new(90);
    public static readonly PartyRoleType Customer = new(91);
    public static readonly PartyRoleType Admin = new(99);


    private PartyRoleType(int value, [CallerMemberName] string? name = null) : base(name, value)
    {
    }
}
