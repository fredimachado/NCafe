using NCafe.Core.Domain;

namespace NCafe.Barista.Domain.Events;

public sealed class OrderPrepared : Event
{
    public OrderPrepared(Guid id)
    {
        Id = id;
    }
}
