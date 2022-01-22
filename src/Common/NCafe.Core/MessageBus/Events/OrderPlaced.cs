namespace NCafe.Core.MessageBus.Events;

public sealed record OrderPlaced(Guid Id, Guid ProductId, int Quantity) : IBusMessage;
