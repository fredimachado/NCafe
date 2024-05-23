using NCafe.Cashier.Domain.ValueObjects;
using NCafe.Core.Domain;

namespace NCafe.Cashier.Domain.Events;

internal sealed record OrderPlaced : Event
{
    public OrderPlaced(Guid id, Customer customer, DateTimeOffset placedAt)
    {
        Id = id;
        Customer = customer;
        PlacedAt = placedAt;
    }

    public Customer Customer { get; }
    public DateTimeOffset PlacedAt { get; }
}
