using Microsoft.Extensions.Configuration;
using ArbTech.Infrastructure.Configuration;

namespace Microsoft.AspNetCore.Hosting;

public static class ConfigurationExtensions
{
    public static string GetRequiredValue(this IConfiguration configuration, string name) =>
        configuration[name] ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}");

    public static string GetRequiredConnectionString(this IConfiguration configuration, string name) =>
        configuration.GetConnectionString(name) ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":ConnectionStrings:" + name : "ConnectionStrings:" + name)}");
    
    
    public static MicroserviceMetaData GetRequiredMicroserviceMetaData(this IConfiguration configuration)
    { 
        var microserviceSection = configuration.GetSection("Microservice");
        
        if (microserviceSection == null)
            throw new InvalidOperationException(
                "Configuration missing 'Microservice: { ApplicationName: , ClientId: , ShortRoute: }' section ");
        
        return new MicroserviceMetaData(
            microserviceSection.GetRequiredValue(nameof(MicroserviceMetaData.ApplicationName)),
            microserviceSection.GetRequiredValue(nameof(MicroserviceMetaData.ClientId))
        );
    }
    
    
}
