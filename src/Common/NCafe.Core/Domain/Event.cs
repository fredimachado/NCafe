using System.Text.Json.Serialization;

namespace NCafe.Core.Domain;

public abstract record Event : IEvent
{
    public Guid Id { get; init; }

    [JsonInclude]
    public long Version { get; internal protected set; }
}
