using NCafe.Core.Domain;

namespace NCafe.Cashier.Api.Projections;

public class ProductCreated : IEvent
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
}
