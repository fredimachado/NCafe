﻿using Ardalis.GuardClauses;
using NCafe.Core.Domain;
using NCafe.Admin.Domain.Events;

namespace NCafe.Admin.Domain.Entities;

public sealed class Product : AggregateRoot
{
    private Product()
    {
    }

    public Product(Guid id, string name, decimal price)
    {
        Id = Guard.Against.Default(id, nameof(id));
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Price = Guard.Against.NegativeOrZero(price, nameof(price));

        RaiseEvent(new ProductCreated(id, name, price));
    }

    public string Name { get; private set; }
    public decimal Price { get; private set; }

    public void Apply(ProductCreated @event)
    {
        Id = @event.Id;
        Name = @event.Name;
        Price = @event.Price;
    }
}
