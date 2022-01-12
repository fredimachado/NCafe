using NCafe.Abstractions.Exceptions;

namespace NCafe.Barista.Domain.Exceptions;

public class OrderNotFoundException : NCafeException
{
    public OrderNotFoundException(Guid orderId) : base($"Order '{orderId}' was not found.")
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}
