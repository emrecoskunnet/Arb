using Microsoft.Extensions.DependencyInjection;

namespace ArbTech.SharedKernel.Messaging;

public interface IEventBus
{ 
    Task PublishAsync(BaseIntegrationEvent integrationEvent); 
}


public interface IEventBusBuilder
{
    public IServiceCollection Services { get; }
}
