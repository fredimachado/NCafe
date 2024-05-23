using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.Messages;
using NCafe.Cashier.Domain.ValueObjects;
using NCafe.Core.Commands;
using NCafe.Core.MessageBus;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Domain.Commands;

public record PlaceOrder(Guid Id, Customer Customer) : ICommand;

internal sealed class PlaceOrderHandler(
    IRepository repository,
    IPublisher publisher,
    TimeProvider timeProvider) : ICommandHandler<PlaceOrder>
{
    private readonly IRepository _repository = repository;
    private readonly IPublisher _publisher = publisher;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task HandleAsync(PlaceOrder command)
    {
        var order = await _repository.GetById<Order>(command.Id) ?? throw new OrderNotFoundException(command.Id); ;

        if (order.Status != OrderStatus.New)
        {
            throw new CannotPlaceOrderException(order.Id);
        }

        if (order.Items.Count == 0)
        {
            throw new CannotPlaceEmptyOrderException(order.Id);
        }

        order.PlaceOrder(command.Customer, _timeProvider.GetUtcNow());

        await _repository.Save(order);

        await _publisher.Publish(
            new OrderPlaced(order.Id, order.Items.Select(x => (OrderPlaced.OrderItem)x).ToArray(), order.Customer));
    }
}
