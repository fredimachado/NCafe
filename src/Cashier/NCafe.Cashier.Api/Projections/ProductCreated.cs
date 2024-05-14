using NCafe.Core.Domain;

namespace NCafe.Cashier.Api.Projections;

public class ProductCreated : Event
{
    public string Name { get; init; }
    public decimal Price { get; init; }
}
