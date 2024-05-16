using NCafe.Core.Exceptions;

namespace NCafe.Cashier.Domain.Exceptions;

public class ProductNotFoundException(Guid productId)
    : DomainException($"Product '{productId}' was not found.")
{
    public Guid ProductId { get; } = productId;
}
