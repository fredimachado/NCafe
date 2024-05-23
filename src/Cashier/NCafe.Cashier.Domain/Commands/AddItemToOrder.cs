using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.ReadModels;
using NCafe.Cashier.Domain.ValueObjects;
using NCafe.Core.Commands;
using NCafe.Core.ReadModels;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Domain.Commands;

public record AddItemToOrder(Guid OrderId, Guid ProductId, int Quantity) : ICommand;

internal sealed class AddItemToOrderHandler(
    IRepository repository,
    IReadModelRepository<Product> productReadRepository) : ICommandHandler<AddItemToOrder>
{
    private readonly IRepository _repository = repository;
    private readonly IReadModelRepository<Product> _productReadRepository = productReadRepository;

    public async Task HandleAsync(AddItemToOrder command)
    {
        var order = await _repository.GetById<Order>(command.OrderId) ?? throw new OrderNotFoundException(command.OrderId);
        var product = _productReadRepository.GetById(command.ProductId) ?? throw new ProductNotFoundException(command.ProductId);

        order.AddItem(new OrderItem(command.ProductId, command.Quantity, product.Name, product.Price));

        await _repository.Save(order);
    }
}
