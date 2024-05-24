using Ardalis.GuardClauses;
using NCafe.Barista.Domain.Events;
using NCafe.Core.Domain;

namespace NCafe.Barista.Domain.Entities;

public sealed class BaristaOrder : AggregateRoot
{
    private BaristaOrder()
    {
    }

    public BaristaOrder(Guid id, ValueObjects.OrderItem[] orderItems, string customer)
    {
        Guard.Against.Default(id);
        Guard.Against.NullOrEmpty(orderItems);
        Guard.Against.NullOrEmpty(customer);

        RaiseEvent(new OrderPlaced(id)
        {
            OrderItems = orderItems,
            Customer = customer
        });
    }

    public IReadOnlyCollection<ValueObjects.OrderItem> Items { get; private set; }
    public string Customer { get; private set; }
    public bool IsCompleted { get; private set; }

    public void CompletePreparation()
    {
        if (IsCompleted)
        {
            return;
        }

        RaiseEvent(new OrderPrepared(Id));
    }

    private void Apply(OrderPlaced @event)
    {
        Id = @event.Id;
        Items = @event.OrderItems;
        Customer = @event.Customer;
        IsCompleted = false;
    }

    private void Apply(OrderPrepared @event)
    {
        IsCompleted = true;
    }
}
