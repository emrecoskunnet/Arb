using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using ArbTech.Infrastructure.DataAccess;
using ArbTech.Infrastructure.Diagnostics;
using ArbTech.SharedKernel.Messaging;

namespace ArbTech.Infrastructure.Messaging.RabbitMq;

public class RabbitMqEventBus(
    ILogger<RabbitMqEventBus> logger,
    IServiceProvider serviceProvider,
    IOptions<EventBusOptions> options,
    IOptions<EventBusSubscriptionInfo> subscriptionOptions,
    RabbitMQTelemetry rabbitMqTelemetry,
    IClaimsPrincipalAdaptor claimsPrincipalAdaptor
    ) : IEventBus, IDisposable, IHostedService
{
    private const string ExchangeName = "SysCore_event_bus";
    private readonly ActivitySource _activitySource = rabbitMqTelemetry.ActivitySource;
    private readonly TextMapPropagator _propagator = rabbitMqTelemetry.Propagator;
    private readonly string _queueName = options.Value.SubscriptionClientName;

    private readonly int _retryCount = options.Value.RetryCount;
    private readonly EventBusSubscriptionInfo _subscriptionInfo = subscriptionOptions.Value;

    private IModel? _consumerChannel;
    private IConnection? _rabbitMqConnection;

    public void Dispose()
    {
        _consumerChannel?.Dispose();
    }

    public Task PublishAsync(BaseIntegrationEvent @event)
    {
        AsyncRetryPolicy? policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        string routingKey = @event.GetType().Name;

        if (logger.IsEnabled(LogLevel.Trace))
            logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id,
                routingKey);

        using IModel channel = _rabbitMqConnection?.CreateModel() ??
                               throw new InvalidOperationException("RabbitMQ connection is not open");

        if (logger.IsEnabled(LogLevel.Trace))
            logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

        channel.ExchangeDeclare(ExchangeName, "direct");

        byte[] body = SerializeMessage(@event);

        // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
        string activityName = $"{routingKey} publish";

        return policy.ExecuteAsync(() =>
        {
            using Activity? activity = _activitySource.StartActivity(activityName, ActivityKind.Client);

            // Depending on Sampling (and whether a listener is registered or not), the activity above may not be created.
            // If it is created, then propagate its context. If it is not created, the propagate the Current context, if any.

            ActivityContext contextToInject = default;

            if (activity != null)
                contextToInject = activity.Context;
            else if (Activity.Current != null) contextToInject = Activity.Current.Context;

            IBasicProperties? properties = channel.CreateBasicProperties();
            // persistent
            properties.DeliveryMode = 2;

            static void InjectTraceContextIntoBasicProperties(IBasicProperties props, string key, string value)
            {
                props.Headers ??= new Dictionary<string, object>();
                props.Headers[key] = value;
            }
            
            _propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), properties,
                InjectTraceContextIntoBasicProperties);

            SetActivityContext(activity, routingKey, "publish");

            if (logger.IsEnabled(LogLevel.Trace)) logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

            try
            {
                channel.BasicPublish(
                    ExchangeName,
                    routingKey,
                    true,
                    properties,
                    body);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                activity?.SetExceptionTags(ex);

                throw;
            }
        });
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Messaging is async so we don't need to wait for it to complete. On top of this
        // the APIs are blocking, so we need to run this on a background thread.
        _ = Task.Factory.StartNew(() =>
            {
                try
                {
                    logger.LogInformation("Starting RabbitMQ connection on a background thread");

                    _rabbitMqConnection = serviceProvider.GetRequiredService<IConnection>();
                    if (!_rabbitMqConnection.IsOpen) return;

                    if (logger.IsEnabled(LogLevel.Trace)) logger.LogTrace("Creating RabbitMQ consumer channel");

                    _consumerChannel = _rabbitMqConnection.CreateModel();

                    _consumerChannel.CallbackException += (sender, ea) =>
                    {
                        logger.LogWarning(ea.Exception, "Error with RabbitMQ consumer channel");
                    };

                    _consumerChannel.ExchangeDeclare(ExchangeName,
                        "direct");

                    _consumerChannel.QueueDeclare(_queueName,
                        true,
                        false,
                        false,
                        null);

                    if (logger.IsEnabled(LogLevel.Trace)) logger.LogTrace("Starting RabbitMQ basic consume");

                    AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                    consumer.Received += OnMessageReceived;

                    _consumerChannel.BasicConsume(
                        _queueName,
                        false,
                        consumer);

                    foreach ((string eventName, Type _) in _subscriptionInfo.EventTypes)
                        _consumerChannel.QueueBind(
                            _queueName,
                            ExchangeName,
                            eventName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error starting RabbitMQ connection");
                }
            },
            TaskCreationOptions.LongRunning);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static void SetActivityContext(Activity? activity, string routingKey, string operation)
    {
        if (activity is null) return;
        // These tags are added demonstrating the semantic conventions of the OpenTelemetry messaging specification
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
        activity.SetTag("messaging.system", "rabbitmq");
        activity.SetTag("messaging.destination_kind", "queue");
        activity.SetTag("messaging.operation", operation);
        activity.SetTag("messaging.destination.name", routingKey);
        activity.SetTag("messaging.rabbitmq.routing_key", routingKey);
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        static IEnumerable<string> ExtractTraceContextFromBasicProperties(IBasicProperties props, string key)
        {
            if (props.Headers.TryGetValue(key, out object? value))
            {
                byte[]? bytes = value as byte[];
                return bytes != null ? [Encoding.UTF8.GetString(bytes)]: [];
            }

            return [];
        }

        // Extract the PropagationContext of the upstream parent from the message headers.
        PropagationContext parentContext =
            _propagator.Extract(default, eventArgs.BasicProperties, ExtractTraceContextFromBasicProperties);
        Baggage.Current = parentContext.Baggage;

        // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
        string activityName = $"{eventArgs.RoutingKey} receive";

        using Activity? activity =
            _activitySource.StartActivity(activityName, ActivityKind.Client, parentContext.ActivityContext);

        SetActivityContext(activity, eventArgs.RoutingKey, "receive");

        string? eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            activity?.SetTag("message", message);

            if (message.Contains("throw-fake-exception", StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");

            await ProcessEvent(eventName, message);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error Processing message \"{Message}\"", message);

            activity?.SetExceptionTags(ex);
        }

        // Even on exception we take the message off the queue.
        // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
        // For more information see: https://www.rabbitmq.com/dlx.html
        _consumerChannel?.BasicAck(eventArgs.DeliveryTag, false);
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (logger.IsEnabled(LogLevel.Trace)) logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

        if (!_subscriptionInfo.EventTypes.TryGetValue(eventName, out Type? eventType))
        {
            logger.LogWarning("Unable to resolve event type for event name {EventName}", eventName);
            return;
        }

        // REVIEW: This could be done in parallel
        var accessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        
        // Get all the handlers using the event type as the key
        foreach (IIntegrationEventHandler handler in scope.ServiceProvider.GetKeyedServices<IIntegrationEventHandler>(eventType))
        {
            // Deserialize the event
            BaseIntegrationEvent? integrationEvent = DeserializeMessage(message, eventType);
            if (integrationEvent != null)
            {
                ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(
                    integrationEvent.Claims?.Select(i => new Claim(i.Type, i.Value)).ToArray(), "Custom"));
                    
                if (accessor.HttpContext != null)
                {
                    if (accessor.HttpContext?.User is not null)
                        accessor.HttpContext.User =  principal;    
                }
                else
                {
                       claimsPrincipalAdaptor.SetClaims(principal);
                }
                
                await handler.Handle(integrationEvent);
            }
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification =
            "The 'JsonSerializer.IsReflectionEnabledByDefault' feature switch, which is set to false by default for trimmed .NET apps, ensures the JsonSerializer doesn't use Reflection.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode", Justification = "See above.")]
    private BaseIntegrationEvent? DeserializeMessage(string message, Type eventType)
    {
        return JsonSerializer.Deserialize(message, eventType, _subscriptionInfo.JsonSerializerOptions) as
            BaseIntegrationEvent;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification =
            "The 'JsonSerializer.IsReflectionEnabledByDefault' feature switch, which is set to false by default for trimmed .NET apps, ensures the JsonSerializer doesn't use Reflection.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode", Justification = "See above.")]
    private byte[] SerializeMessage(BaseIntegrationEvent @event)
    {
        return JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), _subscriptionInfo.JsonSerializerOptions);
    }
}
