namespace NCafe.Abstractions.EventBus.Events;

public sealed record OrderPlaced(Guid Id, Guid ProductId, int Quantity) : IBusMessage;
