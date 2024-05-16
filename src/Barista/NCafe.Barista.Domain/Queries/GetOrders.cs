using NCafe.Barista.Domain.ReadModels;
using NCafe.Core.Queries;
using NCafe.Core.ReadModels;

namespace NCafe.Barista.Domain.Queries;

public record GetOrders : IQuery<BaristaOrder[]>;

internal sealed class GetOrdersHandler(IReadModelRepository<BaristaOrder> orderRepository)
    : IQueryHandler<GetOrders, BaristaOrder[]>
{
    private readonly IReadModelRepository<BaristaOrder> _orderRepository = orderRepository;

    public Task<BaristaOrder[]> HandleAsync(GetOrders query)
    {
        var products = _orderRepository.GetAll()
            .ToArray();
        return Task.FromResult(products);
    }
}
