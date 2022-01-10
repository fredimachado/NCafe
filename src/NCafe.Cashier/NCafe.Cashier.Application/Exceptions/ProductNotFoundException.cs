using NCafe.Abstractions.Exceptions;

namespace NCafe.Cashier.Application.Exceptions;

public class ProductNotFoundException : NCafeException
{
    public ProductNotFoundException(Guid productId) : base($"Product '{productId}' was not found.")
    {
    }
}
