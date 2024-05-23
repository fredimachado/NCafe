using NCafe.Core.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class CannotPlaceEmptyOrderException(Guid orderId)
    : DomainException($"Cannot place order '{orderId}' when item list is empty.")
{
    public Guid OrderId { get; } = orderId;
}
