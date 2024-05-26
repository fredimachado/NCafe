using NCafe.Core.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class OrderItemNotFoundException(Guid orderId, Guid productId)
    : DomainException($"Order item {productId} was not found in order '{orderId}'.")
{
    public Guid OrderId { get; } = orderId;
    public Guid ProductId { get; } = productId;
}
