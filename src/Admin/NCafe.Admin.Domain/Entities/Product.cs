using Ardalis.GuardClauses;
using NCafe.Admin.Domain.Events;
using NCafe.Admin.Domain.Exceptions;
using NCafe.Core.Domain;

namespace NCafe.Admin.Domain.Entities;

public sealed class Product : AggregateRoot
{
    private Product()
    {
    }

    public Product(Guid id, string name, decimal price)
    {
        Id = Guard.Against.Default(id);
        Name = name;
        Price = price;

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidProductNameException();
        }

        if (Price <= 0)
        {
            throw new InvalidProductPriceException(Price);
        }

        RaiseEvent(new ProductCreated(id, name, price));
    }

    public string Name { get; private set; }
    public decimal Price { get; private set; }

    private void Apply(ProductCreated @event)
    {
        Id = @event.Id;
        Name = @event.Name;
        Price = @event.Price;
    }
}
