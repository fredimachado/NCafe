using NCafe.Abstractions.Domain;

namespace NCafe.Abstractions.Exceptions;

public class InvalidVersionException : NCafeException
{
    public InvalidVersionException(IEvent @event, AggregateRoot aggregateRoot)
        : base($"{@event.GetType().Name} Event v{@event.Version} cannot be applied to Aggregate {aggregateRoot.GetType().Name} : {aggregateRoot.Id} v{aggregateRoot.Version}")
    {
    }
}
