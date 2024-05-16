using NCafe.Core.MessageBus.Events;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.ReadModels;
using NCafe.Core.Commands;
using NCafe.Core.MessageBus;
using NCafe.Core.ReadModels;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Domain.Commands;

public record PlaceOrder(Guid ProductId, int Quantity) : ICommand;

internal sealed class PlaceOrderHandler(
    IRepository repository,
    IReadModelRepository<Product> productReadRepository,
    IPublisher publisher) : ICommandHandler<PlaceOrder>
{
    private readonly IRepository _repository = repository;
    private readonly IReadModelRepository<Product> _productReadRepository = productReadRepository;
    private readonly IPublisher _publisher = publisher;

    private const string Topic = "orders";

    public async Task HandleAsync(PlaceOrder command)
    {
        var product = _productReadRepository.GetById(command.ProductId) ?? throw new ProductNotFoundException(command.ProductId);
        var order = new Order(Guid.NewGuid(), product.Id, command.Quantity);

        await _repository.Save(order);

        await _publisher.Publish(Topic, new OrderPlaced(order.Id, order.ProductId, order.Quantity));
    }
}
