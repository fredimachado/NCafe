using NCafe.Core.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class OrderNotFoundException : DomainException
{
    public OrderNotFoundException(Guid orderId) : base($"Order '{orderId}' was not found.")
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}
