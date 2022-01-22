using NCafe.Core.Exceptions;

namespace NCafe.Admin.Domain.Exceptions;

public class InvalidProductPriceException : DomainException
{
    public InvalidProductPriceException(decimal price) : base($"The price '{price}' must be greater than zero.")
    {
    }
}
