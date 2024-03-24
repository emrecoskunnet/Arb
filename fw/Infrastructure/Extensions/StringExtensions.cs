using System.Text;
using System.Text.RegularExpressions;

namespace ArbTech.Infrastructure.Extensions;

public static partial class StringExtensions
{
    public static string ConvertKebabCaseToCamelCase(this string input)
    {
        StringBuilder sb = new();
        bool caseFlag = false;
        foreach (char c in input)
            if (c == '-')
            {
                caseFlag = true;
            }
            else if (caseFlag)
            {
                sb.Append(char.ToUpper(c));
                caseFlag = false;
            }
            else
            {
                sb.Append(char.ToLower(c));
            }

        return sb.ToString();
    }

    public static string ConvertCamelCaseToPascalCase(this string input)
    {
        string output = PascalCaseConverterRegex().Replace(input, match => match.Value.ToUpper());
        return output;
    }

    [GeneratedRegex("\\b\\p{Ll}")]
    private static partial Regex PascalCaseConverterRegex();
}
