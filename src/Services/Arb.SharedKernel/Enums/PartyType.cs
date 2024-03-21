using System.Runtime.CompilerServices;
using Ardalis.SmartEnum;

namespace Arb.SharedKernel.Enums;

public class PartyType: SmartEnum<PartyType>
{
    public static readonly PartyType All = new(0);
    public static readonly PartyType Person = new(1);
    public static readonly PartyType Organization = new(2);
    public static readonly PartyType InternalOrganization = new(3);

    protected PartyType(short value, [CallerMemberName] string? name = null) : base(name, value)
    {
    }
}
