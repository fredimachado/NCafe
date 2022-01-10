using EventStore.Client;
using NCafe.Abstractions.Domain;
using NCafe.Abstractions.Repositories;

namespace NCafe.Infrastructure.EventStore;

public class EventStoreRepository : IRepository
{
    private readonly EventStoreClient eventStoreClient;

    public EventStoreRepository(EventStoreClient eventStoreClient)
    {
        this.eventStoreClient = eventStoreClient;
    }

    public async Task<TAggregate> GetById<TAggregate>(Guid id) where TAggregate : AggregateRoot
    {
        var streamName = ToStreamName(id, typeof(TAggregate));

        var aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), nonPublic: true);

        var result = eventStoreClient.ReadStreamAsync(
            Direction.Forwards,
            streamName,
            StreamPosition.Start);

        var events = await result.ToListAsync();

        foreach (var @event in events)
        {
            var state = @event.AsAggregateEvent();

            aggregate.ApplyEvent(state);
        }

        return aggregate;
    }

    public async Task Save(AggregateRoot aggregate)
    {
        var streamName = ToStreamName(aggregate.Id, aggregate.GetType());
        var pendingEvents = aggregate.GetPendingEvents().ToArray();

        var originalVersion = aggregate.Version - pendingEvents.Length;
        var expectedVersion = originalVersion;

        var eventsToAppend = pendingEvents
            .Select(e => e.AsEventData())
            .ToArray();

        var result = await eventStoreClient.AppendToStreamAsync(
            streamName,
            StreamRevision.FromInt64(expectedVersion),
            eventsToAppend);

        aggregate.ClearPendingEvents();
    }

    private static string ToStreamName(Guid id, Type type)
    {
        return $"{char.ToLower(type.Name[0])}{type.Name.Substring(1)}-{id:N}";
    }
}
