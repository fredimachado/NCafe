using NCafe.Core.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class CannotRemoveItemFromOrderException(Guid orderId, Guid productId)
    : DomainException($"Cannot remove item '{productId}' from order '{orderId}' when status is not New.")
{
    public Guid OrderId { get; } = orderId;
    public Guid ProductId { get; } = productId;
}
