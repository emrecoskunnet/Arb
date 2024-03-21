using System.Runtime.CompilerServices;
using Ardalis.SmartEnum;

namespace Arb.SharedKernel.Enums;

public class LanguageOptionType : SmartEnum<LanguageOptionType>
{
    public static readonly LanguageOptionType All = new(0);
    public static readonly LanguageOptionType Turkish = new(22);
    public static readonly LanguageOptionType English = new(23);
    public static readonly LanguageOptionType Arabic = new(24);

    protected LanguageOptionType(short value, [CallerMemberName] string? name = null) : base(name, value)
    {
    }
}
