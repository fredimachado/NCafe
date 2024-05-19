namespace NCafe.Core.Domain;

public abstract record Event : IEvent
{
    public Guid Id { get; protected set; }

    public long Version { get; internal protected set; }
}
