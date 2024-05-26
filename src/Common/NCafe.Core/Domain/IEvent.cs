namespace NCafe.Core.Domain;

internal interface IEvent
{
    long Version { get; internal protected set; }
}
