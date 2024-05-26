using NCafe.Core.Domain;

namespace NCafe.Cashier.Domain.Events;

public sealed record OrderItemRemoved : Event
{
    public OrderItemRemoved(Guid id, Guid productId, int quantity)
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
    }

    public Guid ProductId { get; }
    public int Quantity { get; }
}
