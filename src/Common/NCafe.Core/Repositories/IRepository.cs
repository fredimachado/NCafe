using NCafe.Core.Domain;

namespace NCafe.Core.Repositories;

public interface IRepository
{
    Task<TAggregate> GetById<TAggregate>(Guid id)
        where TAggregate : AggregateRoot;

    Task Save(AggregateRoot aggregate);
}
