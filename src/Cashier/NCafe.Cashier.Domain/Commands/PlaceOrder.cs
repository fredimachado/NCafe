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

internal sealed class PlaceOrderHandler : ICommandHandler<PlaceOrder>
{
    private readonly IRepository repository;
    private readonly IReadModelRepository<Product> productReadRepository;
    private readonly IPublisher publisher;

    private const string Topic = "orders";

    public PlaceOrderHandler(
        IRepository repository,
        IReadModelRepository<Product> productReadRepository,
        IPublisher publisher)
    {
        this.repository = repository;
        this.productReadRepository = productReadRepository;
        this.publisher = publisher;
    }

    public async Task HandleAsync(PlaceOrder command)
    {
        var product = productReadRepository.GetById(command.ProductId);
        if (product is null)
        {
            throw new ProductNotFoundException(command.ProductId);
        }

        var order = new Order(Guid.NewGuid(), command.ProductId, command.Quantity);

        await repository.Save(order);

        await publisher.Publish(Topic, new OrderPlaced(order.Id, order.ProductId, order.Quantity));
    }
}
