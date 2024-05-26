using MediatR;
using NCafe.Barista.Domain.ReadModels;
using NCafe.Core.ReadModels;

namespace NCafe.Barista.Domain.Queries;

public record GetOrders : IRequest<BaristaOrder[]>;

internal sealed class GetOrdersHandler(IReadModelRepository<BaristaOrder> orderRepository)
    : IRequestHandler<GetOrders, BaristaOrder[]>
{
    private readonly IReadModelRepository<BaristaOrder> _orderRepository = orderRepository;

    public Task<BaristaOrder[]> Handle(GetOrders query, CancellationToken cancellationToken)
    {
        var orders = _orderRepository.GetAll()
            .Where(order => !order.IsCompleted)
            .OrderBy(order => order.OrderPlacedAt)
            .ToArray();
        return Task.FromResult(orders);
    }
}
