using NCafe.Core.Domain;

namespace NCafe.Admin.Domain.Events;

public sealed record ProductDeleted : Event
{
    public ProductDeleted(Guid id)
    {
        Id = id;
    }
}
