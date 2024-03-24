using Microsoft.EntityFrameworkCore.Storage;
using ArbTech.SharedKernel.Messaging;

namespace ArbTech.Infrastructure.Messaging.Outbox.Services;

public interface IIntegrationEventLogService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task SaveEventAsync(BaseIntegrationEvent @event, IDbContextTransaction transaction); 
}
