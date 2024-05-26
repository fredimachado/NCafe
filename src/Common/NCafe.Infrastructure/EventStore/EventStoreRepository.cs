using EventStore.Client;
using Microsoft.Extensions.Logging;
using NCafe.Core.Domain;
using NCafe.Core.Repositories;

namespace NCafe.Infrastructure.EventStore;

internal class EventStoreRepository(EventStoreClient eventStoreClient, ILogger<EventStoreRepository> logger) : IRepository
{
    private readonly EventStoreClient _eventStoreClient = eventStoreClient;
    private readonly ILogger _logger = logger;

    public async Task<TAggregate> GetById<TAggregate>(Guid id) where TAggregate : AggregateRoot
    {
        _logger.LogInformation("Loading aggregate {AggregateType} with ID {AggregateId}", typeof(TAggregate).Name, id);

        var streamName = ToStreamName(id, typeof(TAggregate));

        var aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), nonPublic: true);

        var result = _eventStoreClient.ReadStreamAsync(
            Direction.Forwards,
            streamName,
            StreamPosition.Start);

        var events = await result.ToListAsync();
        _logger.LogTrace("Applying {EventCount} events for aggregate {AggregateType} with ID {AggregateId}",
            events.Count, typeof(TAggregate).Name, id);

        foreach (var @event in events)
        {
            var state = @event.AsAggregateEvent();

            aggregate.ApplyEvent(state);

            _logger.LogTrace("Applied event {EventType} to aggregate {AggregateType} with ID {AggregateId}. Event: {@Event}",
                state.GetType().Name, typeof(TAggregate).Name, id, state);
        }

        return aggregate;
    }

    public async Task Save(AggregateRoot aggregate)
    {
        _logger.LogInformation("Saving aggregate {AggregateType} with ID {AggregateId}", aggregate.GetType().Name, aggregate.Id);

        var streamName = ToStreamName(aggregate.Id, aggregate.GetType());
        var pendingEvents = aggregate.GetPendingEvents().ToArray();

        var expectedVersion = aggregate.Version - pendingEvents.Length;

        var eventsToAppend = pendingEvents
            .Select(e => e.AsEventData())
            .ToArray();

        _logger.LogTrace("Appending {EventCount} events to aggregate {AggregateType} with ID {AggregateId}. Events: {@Events}.",
            eventsToAppend.Length, aggregate.GetType().Name, aggregate.Id, pendingEvents);

        var result = await _eventStoreClient.AppendToStreamAsync(
            streamName,
            StreamRevision.FromInt64(expectedVersion),
            eventsToAppend);

        _logger.LogInformation("Saved aggregate {AggregateType} with ID {AggregateId}. Version: {Version}",
            aggregate.GetType().Name, aggregate.Id, result.NextExpectedStreamRevision.ToInt64());

        aggregate.ClearPendingEvents();
    }

    private static string ToStreamName(Guid id, Type type)
    {
        return $"{char.ToLower(type.Name[0])}{type.Name.AsSpan()[1..]}-{id:N}";
    }
}
