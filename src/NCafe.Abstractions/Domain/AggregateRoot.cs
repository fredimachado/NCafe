using NCafe.Abstractions.Exceptions;

namespace NCafe.Abstractions.Domain;

public abstract class AggregateRoot<TId>
{
    public TId Id { get; protected set; }
    public int Version { get; protected set; } = -1;

    private readonly List<IEvent> _pendingEvents = new();

    public IEnumerable<IEvent> GetPendingEvents()
    {
        return _pendingEvents.ToArray();
    }

    public void ClearPendingEvents()
    {
        _pendingEvents.Clear();
    }

    protected void RaiseEvent(Event @event)
    {
        @event.Version = Version + 1;

        ApplyEvent(@event);

        _pendingEvents.Add(@event);
    }

    public void ApplyEvent(IEvent @event)
    {
        if (@event.Version != Version + 1)
        {
            throw new InvalidVersionException<TId>(@event, this);
        }

        ((dynamic)this).Apply((dynamic)@event);
        Version++;
    }
}
