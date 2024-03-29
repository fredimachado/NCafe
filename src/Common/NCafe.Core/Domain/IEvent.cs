﻿namespace NCafe.Core.Domain;

public interface IEvent
{
    Guid Id { get; }
    long Version { get; }
}

public abstract class Event : IEvent
{
    public Guid Id { get; protected set; }

    public long Version { get; protected internal set; }
}
