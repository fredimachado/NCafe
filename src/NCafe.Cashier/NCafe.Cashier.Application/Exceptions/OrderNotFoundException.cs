using NCafe.Abstractions.Exceptions;

namespace NCafe.Cashier.Application.Exceptions;

public class OrderNotFoundException : NCafeException
{
    public OrderNotFoundException(Guid id) : base($"Order '{id}' was not found.")
    {
    }
}
