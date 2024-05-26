﻿using Ardalis.GuardClauses;
using NCafe.Cashier.Domain.Events;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.ValueObjects;
using NCafe.Core.Domain;

namespace NCafe.Cashier.Domain.Entities;

internal enum OrderStatus
{
    New,
    Canceled,
    Placed,
    Preparing,
    Completed
}

internal sealed class Order : AggregateRoot
{
    private Order()
    {
    }

    private readonly List<OrderItem> _items = [];

    public Order(Guid id, string createdBy, DateTimeOffset createdAt)
    {
        Guard.Against.Default(id);
        Guard.Against.NullOrEmpty(createdBy);
        Guard.Against.Default(createdAt);

        RaiseEvent(new OrderCreated(id, createdBy, createdAt));
    }

    public OrderStatus Status { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset PlacedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Customer Customer { get; private set; }
    public decimal Total { get; private set; }

    public void AddItem(OrderItem orderItem)
    {
        Guard.Against.Null(orderItem);

        if (Status != OrderStatus.New)
        {
            throw new CannotAddItemToOrderException(Id, orderItem.ProductId);
        }

        RaiseEvent(new OrderItemAdded(Id, orderItem.ProductId, orderItem.Quantity, orderItem.Name, orderItem.Price));
    }

    public void RemoveItem(Guid productId, int quantity)
    {
        if (Status != OrderStatus.New)
        {
            throw new CannotRemoveItemFromOrderException(Id, productId);
        }

        var item = Items.FirstOrDefault(i => i.ProductId == productId);

        if (item is null)
        {
            throw new OrderItemNotFoundException(Id, productId);
        }

        if (item.Quantity < quantity)
        {
            throw new CannotRemoveMoreItemsThanOrderedException(Id, productId, quantity);
        }

        RaiseEvent(new OrderItemRemoved(Id, productId, quantity));
    }

    public void PlaceOrder(Customer customer, DateTimeOffset placedAt)
    {
        Guard.Against.Null(customer);

        if (Status != OrderStatus.New)
        {
            throw new CannotPlaceOrderException(Id);
        }

        if (Items.Count == 0)
        {
            throw new CannotPlaceEmptyOrderException(Id);
        }

        RaiseEvent(new OrderPlaced(Id, customer, placedAt));
    }

    private void Apply(OrderCreated @event)
    {
        Id = @event.Id;
        Status = OrderStatus.New;
        CreatedBy = @event.CreatedBy;
        CreatedAt = @event.CreatedAt;
    }

    private void Apply(OrderItemAdded @event)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == @event.ProductId);
        if (item is not null)
        {
            item.IncreaseQuantity(@event.Quantity);
            Total = Items.Sum(i => i.Total);
            return;
        }

        _items.Add(new OrderItem(@event.ProductId, @event.Name, @event.Quantity, @event.Price));
        Total = Items.Sum(i => i.Total);
    }

    private void Apply(OrderItemRemoved @event)
    {
        var item = _items.First(i => i.ProductId == @event.ProductId);
        if (item.Quantity == @event.Quantity)
        {
            _items.Remove(item);
            Total = Items.Sum(i => i.Total);
            return;
        }
        item.DecreaseQuantity(@event.Quantity);
        Total = Items.Sum(i => i.Total);
    }

    private void Apply(OrderPlaced @event)
    {
        Status = OrderStatus.Placed;
        Customer = @event.Customer;
        PlacedAt = @event.PlacedAt;
    }
}
