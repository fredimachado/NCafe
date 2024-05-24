using NCafe.Core.MessageBus;

namespace NCafe.Cashier.Domain.Messages;

public sealed record OrderPlacedMessage(Guid Id, OrderItem[] OrderItems, string CustomerName) : IBusMessage;

public sealed record OrderItem(Guid ProductId, string Name, int Quantity, decimal Price)
{
    public static implicit operator OrderItem(ValueObjects.OrderItem orderItem)
    {
        return new OrderItem(orderItem.ProductId, orderItem.Name, orderItem.Quantity, orderItem.Price);
    }
}
