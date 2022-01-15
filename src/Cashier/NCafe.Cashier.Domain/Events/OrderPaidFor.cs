using NCafe.Abstractions.Domain;

namespace NCafe.Cashier.Domain.Events;

public sealed class OrderPaidFor : Event
{
    public OrderPaidFor(Guid id)
    {
        Id = id;
    }
}
