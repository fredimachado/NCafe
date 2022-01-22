using NCafe.Barista.Domain.Entities;
using NCafe.Barista.Domain.Exceptions;
using NCafe.Core.Commands;
using NCafe.Core.Repositories;

namespace NCafe.Barista.Domain.Commands;

public record CompleteOrder(Guid Id) : ICommand;

internal sealed class CompleteOrderHandler : ICommandHandler<CompleteOrder>
{
    private readonly IRepository repository;

    public CompleteOrderHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task HandleAsync(CompleteOrder command)
    {
        var order = await repository.GetById<BaristaOrder>(command.Id);

        if (order == null)
        {
            throw new OrderNotFoundException(command.Id);
        }

        order.CompletePreparation();

        await repository.Save(order);
    }
}
