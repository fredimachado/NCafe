using NCafe.Abstractions.Commands;
using NCafe.Abstractions.Repositories;
using NCafe.Cashier.Application.Exceptions;
using NCafe.Cashier.Application.Services;
using NCafe.Cashier.Domain.Entities;

namespace NCafe.Cashier.Application.Commands;

public record PlaceOrder(Guid ProductId, int Quantity) : ICommand;

internal sealed class PlaceOrderHandler : ICommandHandler<PlaceOrder>
{
    private readonly IRepository repository;
    private readonly IProductReadService productReadService;

    public PlaceOrderHandler(IRepository repository, IProductReadService productReadService)
    {
        this.repository = repository;
        this.productReadService = productReadService;
    }

    public async Task HandleAsync(PlaceOrder command)
    {
        var product = await productReadService.GetProductAsync(command.ProductId);
        if (product is null)
        {
            throw new ProductNotFoundException(command.ProductId);
        }

        var order = new Order(Guid.NewGuid(), command.ProductId, command.Quantity);

        await repository.Save(order);
    }
}
