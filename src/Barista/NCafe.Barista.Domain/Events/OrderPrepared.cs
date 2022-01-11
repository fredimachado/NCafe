using NCafe.Abstractions.Domain;

namespace NCafe.Barista.Domain.Events;

public class OrderPrepared : Event
{
    public OrderPrepared(Guid id)
    {
        Id = id;
    }
}
