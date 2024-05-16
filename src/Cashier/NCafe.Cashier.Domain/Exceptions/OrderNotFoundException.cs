using NCafe.Core.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class OrderNotFoundException(Guid orderId)
    : DomainException($"Order '{orderId}' was not found.")
{
    public Guid OrderId { get; } = orderId;
}
