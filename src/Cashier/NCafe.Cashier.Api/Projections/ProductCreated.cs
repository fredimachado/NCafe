using NCafe.Core.Domain;

namespace NCafe.Cashier.Api.Projections;

public record ProductCreated : IEvent
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
}
