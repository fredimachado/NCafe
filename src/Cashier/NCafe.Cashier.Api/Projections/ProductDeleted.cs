using NCafe.Core.Domain;

namespace NCafe.Cashier.Api.Projections;

public sealed record ProductDeleted : Event
{
    public ProductDeleted(Guid id)
    {
        Id = id;
    }
}
