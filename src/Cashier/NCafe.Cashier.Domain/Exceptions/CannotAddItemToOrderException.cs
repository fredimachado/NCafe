using NCafe.Core.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;
public class CannotAddItemToOrderException(Guid orderId, Guid productId)
    : DomainException($"Cannot add item '{productId}' to order '{orderId}' when status is not New.")
{
    public Guid OrderId { get; } = orderId;
    public Guid ProductId { get; } = productId;
}
