using NCafe.Abstractions.Domain;

namespace NCafe.Abstractions.Repositories;

public interface IRepository
{
    Task<TAggregate> GetById<TAggregate, TId>(TId id)
        where TAggregate : AggregateRoot<TId>;
    Task Save<TId>(AggregateRoot<TId> aggregate);
}
