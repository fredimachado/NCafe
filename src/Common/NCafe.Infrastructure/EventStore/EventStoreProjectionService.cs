﻿using EventStore.Client;
using Microsoft.Extensions.Logging;
using NCafe.Core.ReadModels;
using System.Text.Json;

namespace NCafe.Infrastructure.EventStore;

internal delegate T TypedEventHandler<T, TEvent>(ResolvedEvent resolvedEvent) where T : class where TEvent : class;

public delegate Guid GetModelId<in TEvent>(TEvent @event);
public delegate void ModelUpdate<in TEvent, T>(TEvent @event, T model);

public sealed class EventStoreProjectionService<T> where T : ReadModel
{
    private readonly EventStoreClient eventStoreClient;
    private readonly IReadModelRepository<T> repository;
    private readonly ILogger logger;

    private readonly Dictionary<string, Func<ResolvedEvent, T>> handlersMap = new();

    public EventStoreProjectionService(
        EventStoreClient eventStoreClient,
        IReadModelRepository<T> repository,
        ILogger<EventStoreProjectionService<T>> logger)
    {
        this.eventStoreClient = eventStoreClient;
        this.repository = repository;
        this.logger = logger;
    }

    public async Task Start(string streamName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(streamName))
        {
            throw new ArgumentException($"Invalid stream name.", nameof(streamName));
        }
        streamName = $"$ce-{streamName}";

        logger.LogInformation("Subscribing to EventStore Stream '{EventStoreStream}'.", streamName);

        await eventStoreClient.SubscribeToStreamAsync(
            streamName,
            EventAppeared,
            resolveLinkTos: true,
            SubscriptionDropped,
            cancellationToken: cancellationToken);

        logger.LogInformation("Subscribed to EventStore Stream '{EventStoreStream}'.", streamName);
    }

    private Task EventAppeared(StreamSubscription subscription, ResolvedEvent @event, CancellationToken cancellationToken)
    {
        if (!handlersMap.TryGetValue(@event.Event.EventType, out var handler))
        {
            logger.LogInformation($"Skipping event '{@event.Event.EventType}' because it is not mapped.");
            return Task.CompletedTask;
        }

        handler(@event);

        return Task.CompletedTask;
    }

    private void SubscriptionDropped(StreamSubscription subscription, SubscriptionDroppedReason reason, Exception exception)
    {
        logger.LogError("Subscription Dropped.");
    }

    public void OnCreate<TEvent>(Func<TEvent, T> handler) where TEvent : class
    {
        MapEventHandler<TEvent>(resolvedEvent =>
        {
            var @event = GetEvent<TEvent>(resolvedEvent);
            var model = handler(@event);

            repository.Add(model);

            return model;
        });
    }

    public void OnUpdate<TEvent>(GetModelId<TEvent> getId, ModelUpdate<TEvent, T> update) where TEvent : class
    {
        MapEventHandler<TEvent>(resolvedEvent =>
        {
            var @event = GetEvent<TEvent>(resolvedEvent);
            var model = repository.GetById(getId(@event));

            update(@event, model);

            repository.Add(model);

            return model;
        });
    }

    private void MapEventHandler<TEvent>(TypedEventHandler<T, TEvent> typedEvent) where TEvent : class
    {
        if (!handlersMap.TryAdd(typeof(TEvent).Name, resolvedEvent => typedEvent(resolvedEvent)))
        {
            throw new ArgumentException($"Event type {typeof(TEvent).Name} already has a handler.");
        }
    }

    private static TEvent GetEvent<TEvent>(ResolvedEvent resolvedEvent)
    {
        return (TEvent)JsonSerializer.Deserialize(resolvedEvent.Event.Data.Span, typeof(TEvent));
    }
}
