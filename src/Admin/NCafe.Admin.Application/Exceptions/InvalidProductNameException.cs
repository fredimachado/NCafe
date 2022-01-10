using NCafe.Abstractions.Exceptions;

namespace NCafe.Admin.Application.Exceptions;

public class InvalidProductNameException : NCafeException
{
    public InvalidProductNameException() : base("The product name must not be null or empty.")
    {
    }
}
