using MediatR;
using NCafe.Barista.Domain.Entities;
using NCafe.Barista.Domain.Exceptions;
using NCafe.Core.Repositories;

namespace NCafe.Barista.Domain.Commands;

public record CompleteOrder(Guid OrderId) : IRequest;

internal sealed class CompleteOrderHandler(IRepository repository) : IRequestHandler<CompleteOrder>
{
    private readonly IRepository _repository = repository;

    public async Task Handle(CompleteOrder command, CancellationToken cancellationToken)
    {
        var order = await _repository.GetById<BaristaOrder>(command.OrderId) ?? throw new OrderNotFoundException(command.OrderId);

        order.CompletePreparation();

        await _repository.Save(order);
    }
}
