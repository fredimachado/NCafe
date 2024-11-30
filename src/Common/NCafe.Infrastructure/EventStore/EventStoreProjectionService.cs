using EventStore.Client;
using Microsoft.Extensions.Logging;
using NCafe.Core.Domain;
using NCafe.Core.Projections;
using NCafe.Core.ReadModels;
using System.Text.Json;

namespace NCafe.Infrastructure.EventStore;

internal delegate T TypedEventHandler<T, TEvent>(ResolvedEvent resolvedEvent) where T : class where TEvent : class, IEvent;

internal sealed class EventStoreProjectionService<T>(
    EventStoreClient eventStoreClient,
    IReadModelRepository<T> repository,
    ILogger<EventStoreProjectionService<T>> logger) : IProjectionService<T> where T : ReadModel
{
    private readonly EventStoreClient _eventStoreClient = eventStoreClient;
    private readonly IReadModelRepository<T> _repository = repository;
    private readonly ILogger _logger = logger;

    private readonly Dictionary<string, Func<ResolvedEvent, T>> _handlersMap = [];

    public async Task Start(CancellationToken cancellationToken)
    {
        var streamName = $"$ce-{JsonNamingPolicy.CamelCase.ConvertName(typeof(T).Name)}";

        _logger.LogInformation("Subscribing to EventStore Stream '{EventStoreStream}'.", streamName);

        // TODO: Add support for checkpointing or use persistent subscription.
        await _eventStoreClient.SubscribeToStreamAsync(
            streamName,
            FromStream.Start,
            EventAppeared,
            resolveLinkTos: true,
            SubscriptionDropped,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Subscribed to EventStore Stream '{EventStoreStream}'.", streamName);
    }

    private Task EventAppeared(StreamSubscription subscription, ResolvedEvent @event, CancellationToken cancellationToken)
    {
        if (!_handlersMap.TryGetValue(@event.Event.EventType, out var handler))
        {
            _logger.LogInformation("Skipping event '{EventType}' because it is not mapped.", @event.Event.EventType);
            return Task.CompletedTask;
        }

        handler(@event);

        return Task.CompletedTask;
    }

    private void SubscriptionDropped(StreamSubscription subscription, SubscriptionDroppedReason reason, Exception exception)
    {
        _logger.LogError("Subscription Dropped for '{EventStoreStream}' with reason '{SubscriptionDroppedReason}'", subscription.SubscriptionId, reason);
        _logger.LogError(exception, "{Exception}", exception.Message);
    }

    public void OnCreate<TEvent>(Func<TEvent, T> handler) where TEvent : Event
    {
        MapEventHandler<TEvent>(resolvedEvent =>
        {
            var @event = GetEvent<TEvent>(resolvedEvent);
            var model = handler(@event);

            _repository.Add(model);

            return model;
        });
    }

    public void OnUpdate<TEvent>(GetModelId<TEvent> getId, ModelUpdate<TEvent, T> update) where TEvent : Event
    {
        MapEventHandler<TEvent>(resolvedEvent =>
        {
            var @event = GetEvent<TEvent>(resolvedEvent);
            var model = _repository.GetById(getId(@event));

            update(@event, model);

            _repository.Add(model);

            return model;
        });
    }

    private void MapEventHandler<TEvent>(TypedEventHandler<T, TEvent> typedEvent) where TEvent : class, IEvent
    {
        if (!_handlersMap.TryAdd(typeof(TEvent).Name, resolvedEvent => typedEvent(resolvedEvent)))
        {
            throw new ArgumentException($"Event type {typeof(TEvent).Name} already has a handler.");
        }
    }

    private static TEvent GetEvent<TEvent>(ResolvedEvent resolvedEvent)
    {
        return (TEvent)JsonSerializer.Deserialize(resolvedEvent.Event.Data.Span, typeof(TEvent));
    }
}
