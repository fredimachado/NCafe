using NCafe.Core.MessageBus;

namespace NCafe.Cashier.Domain.Messages;

public sealed class OrderPlaced(Guid id, OrderPlaced.OrderItem[] orderItems, string customerName) : IBusMessage
{
    public Guid Id { get; } = id;
    public OrderItem[] OrderItems { get; } = orderItems;
    public string CustomerName { get; } = customerName;

    public sealed class OrderItem
    {
        public Guid ProductId { get; init; }
        public string Name { get; init; }
        public int Quantity { get; init; }
        public decimal Price { get; set; }

        public static implicit operator OrderItem(ValueObjects.OrderItem orderItem)
        {
            return new OrderItem
            {
                ProductId = orderItem.ProductId,
                Name = orderItem.Name,
                Quantity = orderItem.Quantity,
                Price = orderItem.Price
            };
        }
    }
}
