using Microsoft.Extensions.DependencyInjection;

namespace ArbTech.Infrastructure.Caching;

public static class Extensions
{ 
    public static void AddCustomBusinessSettingStore(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAppBusinessSettingStore), typeof(DefaultAppBusinessSettingStore)); 
    }
}
