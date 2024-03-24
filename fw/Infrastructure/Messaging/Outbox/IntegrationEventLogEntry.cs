using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using ArbTech.SharedKernel.Messaging;

namespace ArbTech.Infrastructure.Messaging.Outbox;

public class IntegrationEventLogEntry
{
    private static readonly JsonSerializerOptions IndentedOptions = new() { WriteIndented = true };
    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new() { PropertyNameCaseInsensitive = true };

#pragma warning disable CS8618
    // ReSharper disable once UnusedMember.Local
    private IntegrationEventLogEntry() { }
#pragma warning restore CS8618
    public IntegrationEventLogEntry(BaseIntegrationEvent @event, Guid transactionId)
    {
        EventId = @event.Id;
        CreationTime = @event.CreationDate;
        EventTypeName = @event.GetType().FullName 
                        ?? throw new InvalidOperationException();
        Content = JsonSerializer.Serialize(@event, @event.GetType(), IndentedOptions);
        State = EventStateType.NotPublished;
        TimesSent = 0;
        TransactionId = transactionId;
    }
    
    public Guid EventId { get; private set; }
    [Required]
    public string EventTypeName { get; private set; }
    [NotMapped]
    public string? EventTypeShortName => EventTypeName.Split('.')?.Last();
    [NotMapped]
    public BaseIntegrationEvent? IntegrationEvent { get; private set; }
    public EventStateType State { get; set; }
    public int TimesSent { get; set; }
    public DateTime CreationTime { get; private set; }
    [Required]
    public string Content { get; private set; }
    public Guid TransactionId { get; private set; }

    public IntegrationEventLogEntry DeserializeJsonContent(Type type)
    {
        IntegrationEvent = JsonSerializer.Deserialize(Content, type, CaseInsensitiveOptions) as BaseIntegrationEvent;
        return this;
    }
}
