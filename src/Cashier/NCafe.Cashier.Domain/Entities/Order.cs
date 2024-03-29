﻿using Ardalis.GuardClauses;
using NCafe.Cashier.Domain.Events;
using NCafe.Core.Domain;

namespace NCafe.Cashier.Domain.Entities;

public sealed class Order : AggregateRoot
{
    private Order()
    {
    }

    public Order(Guid id, Guid productId, int quantity)
    {
        Guard.Against.Default(id, nameof(id));
        Guard.Against.Default(productId, nameof(productId));
        Guard.Against.NegativeOrZero(quantity, nameof(quantity));

        RaiseEvent(new OrderPlaced(id)
        {
            ProductId = productId,
            Quantity = quantity
        });
    }

    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public bool HasBeenPaid { get; private set; }

    public void PayForOrder()
    {
        RaiseEvent(new OrderPaidFor(Id));
    }

    public void Apply(OrderPlaced @event)
    {
        Id = @event.Id;
        ProductId = @event.ProductId;
        Quantity = @event.Quantity;
        HasBeenPaid = false;
    }

    public void Apply(OrderPaidFor @event)
    {
        HasBeenPaid = true;
    }
}
