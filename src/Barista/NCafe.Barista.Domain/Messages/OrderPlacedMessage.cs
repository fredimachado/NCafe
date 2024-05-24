using NCafe.Core.MessageBus;

namespace NCafe.Barista.Domain.Messages;

[Message("cashier")]
public sealed record OrderPlacedMessage(Guid Id, OrderItem[] OrderItems, string CustomerName) : IBusMessage;

public sealed record OrderItem(Guid ProductId, string Name, int Quantity, decimal Price);
