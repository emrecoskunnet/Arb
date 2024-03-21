using Arb.SharedKernel.Enums;

namespace Arb.SharedKernel.Constants;

public static class DefaultsConstants
{
    public const int ApplicationPartyId = 1; 
    
    public const string PhoneNumberFormat = "{0:0 (###) ### ## ##}";
    
    public static readonly LanguageOptionType DefaultPreferredLanguageId = LanguageOptionType.Turkish;
    public const int DomesticCountryGeoId = 1;
    public const string IdentificationNumberRegex = @"^[1-9]{1}[0-9]{10}$";
    public const string PassportNumberRegex = @"^[1-9]{1,}Z[A-Za-z0-9A-Za-z]{5,}$";

}
