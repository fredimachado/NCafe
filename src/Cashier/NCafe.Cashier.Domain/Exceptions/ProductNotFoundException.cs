using NCafe.Abstractions.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class ProductNotFoundException : NCafeException
{
    public ProductNotFoundException(Guid productId) : base($"Product '{productId}' was not found.")
    {
    }
}
