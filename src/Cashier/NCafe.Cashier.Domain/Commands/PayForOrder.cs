using NCafe.Core.Exceptions;
using NCafe.Cashier.Domain.Entities;
using NCafe.Cashier.Domain.Exceptions;
using NCafe.Core.Commands;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Domain.Commands;

public record PayForOrder(Guid Id) : ICommand;

internal sealed class PayForOrderHandler : ICommandHandler<PayForOrder>
{
    private readonly IRepository repository;

    public PayForOrderHandler(IRepository repository)
    {
        this.repository = repository;
    }

    public async Task HandleAsync(PayForOrder command)
    {
        if (command.Id == Guid.Empty)
        {
            throw new InvalidIdException();
        }

        var order = await repository.GetById<Order>(command.Id);

        if (order is null)
        {
            throw new OrderNotFoundException(command.Id);
        }

        order.PayForOrder();

        await repository.Save(order);
    }
}