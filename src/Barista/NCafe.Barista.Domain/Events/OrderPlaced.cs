using NCafe.Barista.Domain.ValueObjects;
using NCafe.Core.Domain;

namespace NCafe.Barista.Domain.Events;

public sealed record OrderPlaced : Event
{
    public OrderPlaced(Guid id)
    {
        Id = id;
    }

    public OrderItem[] OrderItems { get; init; }
    public string Customer { get; init; }
}
