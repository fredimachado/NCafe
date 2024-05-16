namespace NCafe.Core.Exceptions;

public abstract class DomainException(string message) : Exception(message)
{
}
