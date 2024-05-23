using NCafe.Core.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class CannotPlaceOrderException(Guid orderId)
    : DomainException($"Cannot place order '{orderId}' when status is not New.")
{
    public Guid OrderId { get; } = orderId;
}
