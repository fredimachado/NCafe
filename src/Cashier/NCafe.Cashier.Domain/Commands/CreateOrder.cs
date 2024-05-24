using MediatR;
using NCafe.Cashier.Domain.Entities;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Domain.Commands;

public record CreateOrder(string CreatedBy) : IRequest<Guid>;

internal sealed class CreateOrderHandler(
    IRepository repository,
    TimeProvider timeProvider) : IRequestHandler<CreateOrder, Guid>
{
    private readonly IRepository _repository = repository;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Guid> Handle(CreateOrder command, CancellationToken cancellationToken)
    {
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, command.CreatedBy, _timeProvider.GetUtcNow());

        await _repository.Save(order);

        return orderId;
    }
}
