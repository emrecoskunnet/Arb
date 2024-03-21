using System.Text.RegularExpressions;
using Arb.SharedKernel.Constants;

namespace Arb.SharedKernel.Extensions;

public static class PhoneNumberFormatter
{
    public static string Format(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return phoneNumber;
        
        return long.TryParse(phoneNumber, out long intPhoneNumber) 
            ? string.Format(DefaultsConstants.PhoneNumberFormat, intPhoneNumber) 
            : phoneNumber;
    }

    public static Regex RegexFormat => new Regex("0\\s\\(\\d\\d\\d\\)\\s\\d\\d\\d\\s\\d\\d\\s\\d\\d");
    public static string Clean(string phoneNumber)
        => long.Parse(Regex.Replace(phoneNumber, "[^0-9]", "")).ToString();
}
