using NCafe.Core.Exceptions;

namespace NCafe.Admin.Domain.Exceptions;

public class InvalidProductNameException : DomainException
{
    public InvalidProductNameException() : base("The product name must not be null or empty.")
    {
    }
}
