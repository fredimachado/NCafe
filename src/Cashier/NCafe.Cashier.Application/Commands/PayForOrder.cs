using NCafe.Abstractions.Commands;
using NCafe.Abstractions.Exceptions;
using NCafe.Abstractions.Repositories;
using NCafe.Cashier.Application.Exceptions;
using NCafe.Cashier.Domain.Entities;

namespace NCafe.Cashier.Application.Commands;

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