using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using ArbTech.Infrastructure.Constants;

namespace ArbTech.Infrastructure.Localization;

public class JsonStringLocalizer : IAppLocalizer, IStringLocalizer
{
    private static readonly object SyncRoot = new();
    private ConcurrentBag<JsonLocalizationData> _localization;

    public JsonStringLocalizer()
    {
        string filePath = Path.Combine("Resources", "localization.json");
        if (!Directory.Exists("Resources")) Directory.CreateDirectory("Resources");
        if (!File.Exists(filePath)) File.Create(filePath);

        _localization = new ConcurrentBag<JsonLocalizationData>(
            JsonConvert.DeserializeObject<List<JsonLocalizationData>>(
                File.ReadAllText(filePath))
            ?? new List<JsonLocalizationData>());
    }

    public string this[string name] => GetString(name) ?? name;

    public string this[string name, params object[] arguments]
        => string.Format(this[name], arguments);

    LocalizedString IStringLocalizer.this[string name, params object[] arguments] => new(name, this[name, arguments]);

    LocalizedString IStringLocalizer.this[string name] => new(name, this[name]);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        throw new NotImplementedException();
    }

    private string? GetString(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return name;
        string cultureName = GetCultureKey();
        var query = _localization.Where(l => l.LocalizedValue.Keys.Any(lv => lv == cultureName));
        JsonLocalizationData? value = query.FirstOrDefault(l => l.Key == name);

        if (value == null && Debugger.IsAttached)
        {
            AddNewLocalization(name);
            return name;
        }

        return value?.LocalizedValue[cultureName];
    }

    private static string GetCultureKey(string? cultureName = null)
    {
        return (cultureName ?? (CultureInfo.CurrentCulture.LCID == CultureInfo.InvariantCulture.LCID
            ? DefaultsConstants.DefaultCulture
            : CultureInfo.CurrentCulture.Name))[..2];
    }

    private void AddNewLocalization(string name)
    {
        lock (SyncRoot)
        {
            _localization.Add(new JsonLocalizationData(name)
                { LocalizedValue = new Dictionary<string, string> { { "tr", name }, { "en", name } } });

            File.WriteAllText(Path.Combine("Resources", @"localization.json"),
                JsonConvert.SerializeObject(_localization));

            _localization = new ConcurrentBag<JsonLocalizationData>(
                JsonConvert.DeserializeObject<List<JsonLocalizationData>>(
                    File.ReadAllText(Path.Combine("Resources", @"localization.json")))
                ?? new List<JsonLocalizationData>());
        }
    }
}

internal class JsonLocalizationData
{
    [JsonProperty("Values")] public Dictionary<string, string> LocalizedValue = new();

    public JsonLocalizationData(string key)
    {
        Key = key;
    }

    public string Key { get; init; }
}
