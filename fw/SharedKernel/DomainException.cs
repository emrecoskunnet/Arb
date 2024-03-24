using System.Runtime.Serialization;

namespace ArbTech.SharedKernel;

public class DomainException : Exception
{
    public string DomainMessage { get; private set; }
    public List<string> MessageParams { get; private set; }


    protected DomainException(string message, List<string> messageParams)
        : base(string.Format(message, messageParams.ToArray<object>()))
    {
        DomainMessage = message;
        MessageParams = messageParams;
    }

    private readonly Dictionary<string, List<string>> _metadata = new();

    public IEnumerable<(string propertyName, List<string> values)> Metadata =>
        _metadata.Select(i => (i.Key, i.Value.ToList()));

    protected void AddMetadata(string propertyName, string? value)
    {
        if(string.IsNullOrWhiteSpace(value)) return;
        if (_metadata.TryGetValue(propertyName, out var list))
            list.Add(value);
        else
            _metadata.Add(propertyName, new List<string> { value });
    }
}
