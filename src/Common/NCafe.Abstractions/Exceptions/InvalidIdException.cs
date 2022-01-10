namespace NCafe.Abstractions.Exceptions;

public class InvalidIdException : NCafeException
{
    public InvalidIdException()
        : base("Invalid Id.")
    {
    }
}
