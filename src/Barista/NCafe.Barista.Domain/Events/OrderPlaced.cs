using NCafe.Abstractions.Domain;

namespace NCafe.Barista.Domain.Events;

public sealed class OrderPlaced : Event
{
    public OrderPlaced(Guid id)
    {
        Id = id;
    }

    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
