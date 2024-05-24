using MediatR;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.Messages;
using NCafe.Cashier.Domain.ValueObjects;
using NCafe.Core.MessageBus;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Domain.Commands;

public record PlaceOrder(Guid OrderId, Customer Customer) : IRequest;

internal sealed class PlaceOrderHandler(
    IRepository repository,
    IBusPublisher publisher,
    TimeProvider timeProvider) : IRequestHandler<PlaceOrder>
{
    private readonly IRepository _repository = repository;
    private readonly IBusPublisher _publisher = publisher;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task Handle(PlaceOrder command, CancellationToken cancellationToken)
    {
        var order = await _repository.GetById<Order>(command.OrderId) ?? throw new OrderNotFoundException(command.OrderId);

        order.PlaceOrder(command.Customer, _timeProvider.GetUtcNow());

        await _repository.Save(order);

        await _publisher.Publish(
            new OrderPlacedMessage(order.Id, order.Items.Select(x => (Messages.OrderItem)x).ToArray(), order.Customer));
    }
}
