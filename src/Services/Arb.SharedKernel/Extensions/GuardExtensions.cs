using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Ardalis.GuardClauses;

namespace Arb.SharedKernel.Extensions;

public static partial class GuardExtensions
{
    public static string InvalidIdentificationNumber(this IGuardClause guardClause,
        [NotNull] [ValidatedNotNull] string? identificationNumber,
        [CallerArgumentExpression("identificationNumber")]
        string? parameterName = null,
        string? message = null)
    {
        Guard.Against.NullOrWhiteSpace(identificationNumber);

        Regex regex = NumericOnlyRegex();
        if (!regex.IsMatch(identificationNumber))
        {
            throw new ArgumentException(message ?? $"Input {parameterName} must consists of numeric only.", parameterName);
        }

        var firstCheck = Convert.ToInt32(identificationNumber[0].ToString()) +
                         Convert.ToInt32(identificationNumber[2].ToString()) +
                         Convert.ToInt32(identificationNumber[4].ToString()) +
                         Convert.ToInt32(identificationNumber[6].ToString()) +
                         Convert.ToInt32(identificationNumber[8].ToString());
        var secondCheck = Convert.ToInt32(identificationNumber[1].ToString()) +
                          Convert.ToInt32(identificationNumber[3].ToString()) +
                          Convert.ToInt32(identificationNumber[5].ToString()) +
                          Convert.ToInt32(identificationNumber[7].ToString());

        var result = firstCheck * 7;
        result -= secondCheck;
        result %= 10;

        if (identificationNumber.Length < 10 || result.ToString() != identificationNumber[9].ToString())
        {
            throw new ArgumentException(message ?? $"Input {parameterName} is incompatible with TC ID number algorithm.", parameterName);
        }

        var sum3 = 0;
        for (var i = 0; i < 10; i++)
        {
            sum3 += Convert.ToInt32(identificationNumber[i].ToString());
        }

        if ((sum3 % 10).ToString() != identificationNumber[10].ToString())
        {
            throw new ArgumentException(message ?? $"Input {parameterName} is incompatible with TC ID number algorithm.", parameterName);
        } 
        return identificationNumber;
    }

    [GeneratedRegex(@"^[0-9]*$")]
    private static partial Regex NumericOnlyRegex();
}
