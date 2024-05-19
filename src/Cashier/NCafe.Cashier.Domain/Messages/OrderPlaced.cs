using NCafe.Core.MessageBus;

namespace NCafe.Cashier.Domain.Messages;

public sealed record OrderPlaced(Guid Id, Guid ProductId, int Quantity) : IBusMessage;
