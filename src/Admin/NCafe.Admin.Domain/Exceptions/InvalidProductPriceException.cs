using NCafe.Abstractions.Exceptions;

namespace NCafe.Admin.Domain.Exceptions;

public class InvalidProductPriceException : NCafeException
{
    public InvalidProductPriceException(decimal price) : base($"The price '{price}' must be greater than zero.")
    {
    }
}
