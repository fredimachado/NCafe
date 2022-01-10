using NCafe.Abstractions.Domain;

namespace NCafe.Admin.Domain.Events;

public sealed class ProductCreated : Event
{
    public ProductCreated(Guid id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
    }

    public string Name { get; }
    public decimal Price { get; }
}
