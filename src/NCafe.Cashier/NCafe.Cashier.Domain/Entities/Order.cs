using Ardalis.GuardClauses;
using NCafe.Abstractions.Domain;
using NCafe.Cashier.Domain.Events;

namespace NCafe.Cashier.Domain.Entities
{
    public sealed class Order : AggregateRoot<Guid>
    {
        private Order()
        {
        }

        public Order(Guid id, Guid productId, int quantity)
        {
            Id = Guard.Against.Default(id, nameof(id));
            productId = Guard.Against.Default(productId, nameof(productId));
            Quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));

            RaiseEvent(new OrderPlaced(Id)
            {
                ProductId = productId,
                Quantity = quantity
            });
        }

        public Guid ProductId { get; set; }
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
}
