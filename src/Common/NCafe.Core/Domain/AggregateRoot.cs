using NCafe.Core.Exceptions;
using ReflectionMagic;

namespace NCafe.Core.Domain;

public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }
    internal long Version { get; set; } = -1;

    private readonly List<Event> _pendingEvents = [];

    protected void RaiseEvent(Event @event)
    {
        ((IEvent)@event).Version = Version + 1;

        ApplyEvent(@event);

        _pendingEvents.Add(@event);
    }

    public IReadOnlyCollection<Event> GetPendingEvents()
    {
        return _pendingEvents.AsReadOnly();
    }

    internal void ClearPendingEvents()
    {
        _pendingEvents.Clear();
    }

    internal void ApplyEvent(Event @event)
    {
        if (((IEvent)@event).Version != Version + 1)
        {
            throw new InvalidVersionException(@event, this);
        }

        this.AsDynamic().Apply(@event);
        Version++;
    }
}
