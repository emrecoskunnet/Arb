using System.Text.RegularExpressions;

namespace ArbTech.Infrastructure.Tools;

public static partial class CsvFileReader
{
    public static (Dictionary<string, int> columns, IEnumerable<string[]> rows) ReadCsvFile(string fullPath, string[] requiredHeaders, string[]? optionalHeaders = null)
    { 
        if (!File.Exists(fullPath))
        {
            throw new InvalidOperationException(@$"Customization data not found {fullPath}");
        }
        return (
            GetHeaders(fullPath, requiredHeaders, optionalHeaders),
             File.ReadAllLines(fullPath)
                .Skip(1) // skip header row
                .Select(row => MyRegex().Split(row)));
    }
    
    private static Dictionary<string, int> GetHeaders(string csvFile, IReadOnlyCollection<string> requiredHeaders, IReadOnlyCollection<string>? optionalHeaders = null)
    {
        string[] csvHeaders = File.ReadLines(csvFile).First().ToLowerInvariant().Split(',');

        if (csvHeaders.Length < requiredHeaders.Count)
        {
            throw new InvalidOperationException($@"requiredHeader count '{requiredHeaders.Count}' is bigger then csv header count '{csvHeaders.Length}' ");
        }

        if (optionalHeaders != null && csvHeaders.Length > requiredHeaders.Count + optionalHeaders.Count)
        {
            throw new InvalidOperationException($"csv header count '{csvHeaders.Length}'  is larger then required '{requiredHeaders.Count}' and optional '{optionalHeaders.Count}' headers count");
        }

        foreach (string requiredHeader in requiredHeaders)
        {
            if (!csvHeaders.Contains(requiredHeader))
            {
                throw new InvalidOperationException($"does not contain required header '{requiredHeader}'");
            }
        }

        var result = csvHeaders.Select((i,x) => new { value =i.Trim('"').Trim(), id = x });
        return result.ToDictionary(i => i.value, i => i.id);
    }

    [GeneratedRegex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")]
    private static partial Regex MyRegex();
}
