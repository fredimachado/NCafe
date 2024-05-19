using NCafe.Core.Domain;

namespace NCafe.Barista.Domain.Events;

public sealed record OrderPrepared : Event
{
    public OrderPrepared(Guid id)
    {
        Id = id;
    }
}
