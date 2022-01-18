using NCafe.Abstractions.Exceptions;

namespace NCafe.Abstractions.Domain;

public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }
    public long Version { get; protected set; } = -1;

    private readonly List<IEvent> _pendingEvents = new();

    protected void RaiseEvent(Event @event)
    {
        @event.Version = Version + 1;

        ApplyEvent(@event);

        _pendingEvents.Add(@event);
    }

    internal IEnumerable<IEvent> GetPendingEvents()
    {
        return _pendingEvents.ToArray();
    }

    internal void ClearPendingEvents()
    {
        _pendingEvents.Clear();
    }

    internal void ApplyEvent(IEvent @event)
    {
        if (@event.Version != Version + 1)
        {
            throw new InvalidVersionException(@event, this);
        }

        ((dynamic)this).Apply((dynamic)@event);
        Version++;
    }
}
