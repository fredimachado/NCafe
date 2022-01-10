namespace NCafe.Abstractions.Exceptions;

public abstract class NCafeException : Exception
{
    protected NCafeException(string message) : base(message)
    {
    }
}
