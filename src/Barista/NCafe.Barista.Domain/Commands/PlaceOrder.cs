using MediatR;
using NCafe.Barista.Domain.Entities;
using NCafe.Core.Repositories;

namespace NCafe.Barista.Domain.Commands;

public record PlaceOrder(Guid Id, OrderItem[] OrderItems, string Customer) : IRequest;
public record OrderItem(Guid ProductId, string Name, int Quantity, decimal Price);

internal sealed class PlaceOrderHandler(IRepository repository) : IRequestHandler<PlaceOrder>
{
    private readonly IRepository _repository = repository;

    public async Task Handle(PlaceOrder command, CancellationToken cancellationToken)
    {
        var order = new BaristaOrder(
            command.Id,
            command.OrderItems.Select(i => new ValueObjects.OrderItem(i.ProductId, i.Name, i.Quantity, i.Price)).ToArray(),
            command.Customer);

        await _repository.Save(order);
    }
}
