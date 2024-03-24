using System.Security.Claims;
using System.Text.Json.Serialization;

namespace ArbTech.SharedKernel.Messaging;

public record BaseIntegrationEvent
{
    [JsonInclude]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonInclude]
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    [JsonInclude] public ICollection<ClaimInfo>? Claims { get; set; } = null;
}

public record ClaimInfo
{
    [JsonInclude] public string Type { get; set; } = string.Empty;
    [JsonInclude]
    public string Value { get; set; }= string.Empty;
}
