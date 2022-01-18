using EventStore.Client;
using NCafe.Abstractions.Domain;
using System.Text;
using System.Text.Json;

namespace NCafe.Infrastructure.EventStore;

internal static class EventExtensions
{
    private const string EventClrTypeNameHeader = "EventClrTypeName";

    public static Event AsAggregateEvent(this ResolvedEvent resolvedEvent)
    {
        var eventClrTypeName = JsonDocument.Parse(resolvedEvent.Event.Metadata)
            .RootElement
            .GetProperty(EventClrTypeNameHeader)
            .GetString();

        if (JsonSerializer.Deserialize(resolvedEvent.Event.Data.Span, Type.GetType(eventClrTypeName)) is Event @event)
        {
            return @event;
        }

        throw new ApplicationException($"Could not deserialise {eventClrTypeName} as an Event (metadata: {Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray())}");
    }

    public static EventData AsEventData(this IEvent @event)
    {
        return new EventData(Uuid.NewUuid(), @event.GetType().Name, Serialize(@event), SerializeEventMetadata(@event));
    }

    private static byte[] SerializeEventMetadata(IEvent @event)
    {
        var metadata = new Dictionary<string, string>
        {
            { EventClrTypeNameHeader, @event.GetType().AssemblyQualifiedName }
        };

        return Serialize(metadata);
    }

    private static byte[] Serialize(object value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, value.GetType());
    }
}
