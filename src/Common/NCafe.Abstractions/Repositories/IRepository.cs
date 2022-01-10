using NCafe.Abstractions.Domain;

namespace NCafe.Abstractions.Repositories;

public interface IRepository
{
    Task<TAggregate> GetById<TAggregate>(Guid id)
        where TAggregate : AggregateRoot;
    Task Save<TId>(AggregateRoot aggregate);
}
