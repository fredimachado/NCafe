using NCafe.Core.MessageBus;

namespace NCafe.Barista.Domain.Messages;

[Message("cashier")]
public sealed record OrderPlaced(Guid Id, Guid ProductId, int Quantity) : IBusMessage;
