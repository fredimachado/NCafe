using NCafe.Core.Domain;

namespace NCafe.Core.Exceptions;

public class InvalidVersionException(Event @event, AggregateRoot aggregateRoot)
    : DomainException($"{@event.GetType().Name} Event v{@event.Version} cannot be applied to Aggregate {aggregateRoot.GetType().Name} : {aggregateRoot.Id} v{aggregateRoot.Version}")
{
}
