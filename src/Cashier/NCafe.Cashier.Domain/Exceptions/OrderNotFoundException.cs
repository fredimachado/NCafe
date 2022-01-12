using NCafe.Abstractions.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class OrderNotFoundException : NCafeException
{
    public OrderNotFoundException(Guid id) : base($"Order '{id}' was not found.")
    {
    }
}
