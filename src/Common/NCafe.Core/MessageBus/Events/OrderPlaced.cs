namespace NCafe.Core.MessageBus.Events;

[Message("cashier")]
public sealed record OrderPlaced(Guid Id, Guid ProductId, int Quantity) : IBusMessage;
