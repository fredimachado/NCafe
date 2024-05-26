using MediatR;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Cashier.Domain.ReadModels;
using NCafe.Cashier.Domain.ValueObjects;
using NCafe.Core.ReadModels;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Domain.Commands;

public record RemoveItemFromOrder(Guid OrderId, Guid ProductId, int Quantity) : IRequest;

internal sealed class RemoveItemFromOrderHandler(IRepository repository) : IRequestHandler<RemoveItemFromOrder>
{
    private readonly IRepository _repository = repository;

    public async Task Handle(RemoveItemFromOrder command, CancellationToken cancellationToken)
    {
        var order = await _repository.GetById<Order>(command.OrderId) ?? throw new OrderNotFoundException(command.OrderId);

        order.RemoveItem(command.ProductId, command.Quantity);

        await _repository.Save(order);
    }
}
