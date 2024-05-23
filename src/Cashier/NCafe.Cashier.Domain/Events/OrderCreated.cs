using NCafe.Core.Domain;

namespace NCafe.Cashier.Domain.Events;

internal sealed record OrderCreated : Event
{
    public OrderCreated(Guid id, string createdBy, DateTimeOffset createdAt)
    {
        Id = id;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public string CreatedBy { get; }
    public DateTimeOffset CreatedAt { get; }
}
