using NCafe.Cashier.Domain.Entities;
using NCafe.Core.Commands;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Domain.Commands;

public record CreateOrder(string CreatedBy) : ICommand;

internal sealed class CreateOrderHandler(
    IRepository repository,
    TimeProvider timeProvider) : ICommandHandler<CreateOrder>
{
    private readonly IRepository _repository = repository;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task HandleAsync(CreateOrder command)
    {
        var order = new Order(Guid.NewGuid(), command.CreatedBy, _timeProvider.GetUtcNow());

        await _repository.Save(order);
    }
}
