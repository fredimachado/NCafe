using NCafe.Core.Exceptions;

namespace NCafe.Admin.Domain.Exceptions;

public class InvalidProductPriceException(decimal price)
    : DomainException($"The price '{price}' must be greater than zero.")
{
}
