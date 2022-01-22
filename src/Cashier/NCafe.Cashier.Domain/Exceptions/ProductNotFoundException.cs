using NCafe.Core.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(Guid productId) : base($"Product '{productId}' was not found.")
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
