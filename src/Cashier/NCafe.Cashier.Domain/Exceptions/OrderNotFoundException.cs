using NCafe.Abstractions.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class OrderNotFoundException : NCafeException
{
    public OrderNotFoundException(Guid orderId) : base($"Order '{orderId}' was not found.")
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}
