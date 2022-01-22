using NCafe.Core.Domain;

namespace NCafe.Core.Exceptions;

public class InvalidVersionException : DomainException
{
    public InvalidVersionException(IEvent @event, AggregateRoot aggregateRoot)
        : base($"{@event.GetType().Name} Event v{@event.Version} cannot be applied to Aggregate {aggregateRoot.GetType().Name} : {aggregateRoot.Id} v{aggregateRoot.Version}")
    {
    }
}
