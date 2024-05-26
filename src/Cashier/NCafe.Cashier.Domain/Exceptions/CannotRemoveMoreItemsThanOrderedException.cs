using NCafe.Core.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class CannotRemoveMoreItemsThanOrderedException(Guid orderId, Guid productId, int quantity)
    : DomainException($"Cannot remove more items ({quantity}) than existing in the order '{orderId}'.")
{
    public Guid OrderId { get; } = orderId;
    public Guid ProductId { get; } = productId;
    public int Quantity { get; } = quantity;
}
