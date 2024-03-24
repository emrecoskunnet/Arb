using Microsoft.Extensions.DependencyInjection;
using ArbTech.Infrastructure.Messaging.Outbox.Services;

namespace ArbTech.Infrastructure.DataAccess;

public static class DataAccessExtensions
{ 
    public static void AddCustomArdalisRepository(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddSingleton<IClaimsPrincipalAdaptor, DefaultClaimsPrincipalAdaptor>();
        // services.AddScoped(typeof(IIntegrationRepository<>), typeof(IntegrationRepository<>));
        // services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService>();
    }
    
    
}
