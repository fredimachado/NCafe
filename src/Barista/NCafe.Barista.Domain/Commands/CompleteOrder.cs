using NCafe.Barista.Domain.Entities;
using NCafe.Barista.Domain.Exceptions;
using NCafe.Core.Commands;
using NCafe.Core.Repositories;

namespace NCafe.Barista.Domain.Commands;

public record CompleteOrder(Guid Id) : ICommand;

internal sealed class CompleteOrderHandler(IRepository repository) : ICommandHandler<CompleteOrder>
{
    private readonly IRepository _repository = repository;

    public async Task HandleAsync(CompleteOrder command)
    {
        var order = await _repository.GetById<BaristaOrder>(command.Id) ?? throw new OrderNotFoundException(command.Id);

        order.CompletePreparation();

        await _repository.Save(order);
    }
}
