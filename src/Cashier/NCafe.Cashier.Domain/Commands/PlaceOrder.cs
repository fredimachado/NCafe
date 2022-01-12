using NCafe.Abstractions.Commands;
using NCafe.Abstractions.MessageBus;
using NCafe.Abstractions.MessageBus.Events;
using NCafe.Abstractions.ReadModels;
using NCafe.Abstractions.Repositories;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.ReadModels;

namespace NCafe.Cashier.Domain.Commands;

public record PlaceOrder(Guid ProductId, int Quantity) : ICommand;

internal sealed class PlaceOrderHandler : ICommandHandler<PlaceOrder>
{
    private readonly IRepository repository;
    private readonly IReadModelRepository<Product> productReadRepository;
    private readonly IPublisher publisher;

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

        await publisher.Publish("orders", new OrderPlaced(order.Id, order.ProductId, order.Quantity));
    }
}
