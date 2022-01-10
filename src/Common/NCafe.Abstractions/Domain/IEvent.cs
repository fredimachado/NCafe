namespace NCafe.Abstractions.Domain;

public interface IEvent
{
    Guid Id { get; }
    int Version { get; }
}

public abstract class Event : IEvent
{
    public Guid Id { get; protected set; }

    public int Version { get; protected internal set; }
}