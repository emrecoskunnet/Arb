using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using ArbTech.Infrastructure.DataAccess;
using ArbTech.SharedKernel.Messaging;

namespace ArbTech.Infrastructure.Messaging.Outbox.Services;

public sealed class IntegrationEventLogService : IIntegrationEventLogService, IDisposable
{
    private volatile bool _disposedValue;
    private readonly ArbTechDbContext _context;
    private readonly IEventBus _eventBus;
    private readonly ILogger<IntegrationEventLogService> _logger;
    private readonly Type[] _eventTypes;

    public IntegrationEventLogService(ArbTechDbContext context, IEventBus eventBus,
        ILogger<IntegrationEventLogService> logger)
    {
        _context = context;
        _eventBus = eventBus;
        _logger = logger;
        Type integrationEventType = typeof(BaseIntegrationEvent);
        _eventTypes = Assembly.GetEntryAssembly()!
            .GetTypes()
            .Where(t => integrationEventType.IsAssignableFrom(t)
                        || t.IsAssignableFrom(integrationEventType))
            .ToArray();
    }

    private async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
    {
        var result = await _context.Set<IntegrationEventLogEntry>()
            .Where(e => e.TransactionId == transactionId && e.State == EventStateType.NotPublished)
            .ToListAsync();

        if (result.Count != 0)
        {
            return result.OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(
                    _eventTypes.First(t => t.Name == e.EventTypeShortName)
                    ?? throw new InvalidOperationException())).ToList();
        }

        return [];
    }

    public Task SaveEventAsync(BaseIntegrationEvent @event, IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        var eventLogEntry = new IntegrationEventLogEntry(@event, transaction.TransactionId);

        _context.Database.UseTransaction(transaction.GetDbTransaction());
        _context.Set<IntegrationEventLogEntry>().Add(eventLogEntry);

        return _context.SaveChangesAsync();
    }

    private Task MarkEventAsPublishedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, EventStateType.Published);
    }

    private Task MarkEventAsInProgressAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, EventStateType.InProgress);
    }

    private Task MarkEventAsFailedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, EventStateType.PublishedFailed);
    }

    public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
    {
        IEnumerable<IntegrationEventLogEntry> pendingLogEvents =
            await RetrieveEventLogsPendingToPublishAsync(transactionId);

        foreach (IntegrationEventLogEntry logEvt in pendingLogEvents)
        {
            _logger.LogInformation("Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})",
                logEvt.EventId, logEvt.IntegrationEvent);

            try
            {
                await MarkEventAsInProgressAsync(logEvt.EventId);
                await _eventBus.PublishAsync(logEvt.IntegrationEvent ?? throw new InvalidOperationException());
                await MarkEventAsPublishedAsync(logEvt.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing integration event: {IntegrationEventId}", logEvt.EventId);

                await MarkEventAsFailedAsync(logEvt.EventId);
            }
        }
    }

    private Task UpdateEventStatus(Guid eventId, EventStateType status)
    {
        var eventLogEntry = _context.Set<IntegrationEventLogEntry>().Single(ie => ie.EventId == eventId);
        eventLogEntry.State = status;

        if (status == EventStateType.InProgress)
            eventLogEntry.TimesSent++;

        return _context.SaveChangesAsync();
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _context.Dispose();
            }


            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
