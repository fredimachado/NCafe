using Ardalis.GuardClauses;
using NCafe.Abstractions.Domain;
using NCafe.Barista.Domain.Events;

namespace NCafe.Barista.Domain.Entities;

public sealed class BaristaOrder : AggregateRoot
{
    private BaristaOrder()
    {
    }

    public BaristaOrder(Guid id, Guid productId, int quantity)
    {
        Guard.Against.Default(id, nameof(id));
        Guard.Against.Default(productId, nameof(productId));
        Guard.Against.Negative(quantity, nameof(quantity));

        RaiseEvent(new OrderPlaced(id)
        {
            ProductId = productId,
            Quantity = quantity
        });
    }

    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public bool IsCompleted { get; private set; }

    public void CompletePreparation()
    {
        RaiseEvent(new OrderPrepared(Id));
    }

    public void Apply(OrderPlaced @event)
    {
        Id = @event.Id;
        ProductId = @event.ProductId;
        Quantity = @event.Quantity;
        IsCompleted = false;
    }

    public void Apply(OrderPrepared @event)
    {
        IsCompleted = true;
    }
}
