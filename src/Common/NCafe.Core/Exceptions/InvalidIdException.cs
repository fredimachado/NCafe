namespace NCafe.Core.Exceptions;

public class InvalidIdException : DomainException
{
    public InvalidIdException()
        : base("Invalid Id.")
    {
    }
}
