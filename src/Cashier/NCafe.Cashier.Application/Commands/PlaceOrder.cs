using NCafe.Abstractions.Commands;
using NCafe.Abstractions.ReadModels;
using NCafe.Abstractions.Repositories;
using NCafe.Cashier.Application.Exceptions;
using NCafe.Cashier.Application.ReadModels;
using NCafe.Cashier.Domain.Entities;

namespace NCafe.Cashier.Application.Commands;

public record PlaceOrder(Guid ProductId, int Quantity) : ICommand;

internal sealed class PlaceOrderHandler : ICommandHandler<PlaceOrder>
{
    private readonly IRepository repository;
    private readonly IReadModelRepository<Product> productReadRepository;

    public PlaceOrderHandler(IRepository repository, IReadModelRepository<Product> productReadRepository)
    {
        this.repository = repository;
        this.productReadRepository = productReadRepository;
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
    }
}
