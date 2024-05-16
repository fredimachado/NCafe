namespace NCafe.Core.Domain;

public abstract class Event : IEvent
{
    public Guid Id { get; protected set; }

    public long Version { get; internal protected set; }
}
