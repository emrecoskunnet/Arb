using System.Text;
using System.Text.Json;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using ArbTech.Infrastructure.Configuration;

namespace ArbTech.Infrastructure.Caching;

public class DefaultAppBusinessSettingStore(IDistributedCache appCacheService, IConfiguration configuration)
    : IAppBusinessSettingStore
{
    /// BusinessSettings: {
    ///     Key1: true,
    ///     Key2: 2
    /// ...
    public async Task<T?> GetConfiguration<T>(string settingKey,
        CancellationToken cancellationToken = default)
    {
        MicroserviceMetaData microservice = configuration.GetRequiredMicroserviceMetaData();

        string key = @$"{microservice.ClientId}_{settingKey}";
        string? value = await appCacheService.GetStringAsync(key, cancellationToken);
        if (value == null)
        {
            var businessSettingSection = configuration.GetRequiredSection("BusinessSettings");
            Guard.Against.Null(businessSettingSection);
            value = businessSettingSection.GetRequiredValue(settingKey);

            await SetConfiguration(settingKey, value, cancellationToken);
        }
        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetConfiguration<T>(string settingKey, T? value, 
        CancellationToken cancellationToken = default)
    {
        MicroserviceMetaData microservice = configuration.GetRequiredMicroserviceMetaData();
        string clientId = microservice.ClientId;

        string key = @$"{clientId}_{settingKey}";
        await appCacheService.SetAsync(key, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)), new DistributedCacheEntryOptions()
        {
            AbsoluteExpiration = DateTime.Now.AddSeconds(180)
        }, cancellationToken);
    }
}
 
