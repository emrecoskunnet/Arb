namespace ArbTech.Infrastructure.Configuration;

public record MicroserviceMetaData(string ApplicationName, string ClientId)
{
    public string ApplicationName { get; } = ApplicationName;
    public string ClientId { get; } = ClientId;
}
