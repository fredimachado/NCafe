using NCafe.Abstractions.Domain;

namespace NCafe.Abstractions.Exceptions;

public class InvalidVersionException<T> : NCafeException
{
    public InvalidVersionException(IEvent @event, AggregateRoot<T> aggregateRoot)
        : base($"{@event.GetType().Name} Event v{@event.Version} cannot be applied to Aggregate {aggregateRoot.GetType().Name} : {aggregateRoot.Id} v{aggregateRoot.Version}")
    {
    }
}
