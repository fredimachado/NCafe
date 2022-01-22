using NCafe.Barista.Domain.ReadModels;
using NCafe.Core.Queries;
using NCafe.Core.ReadModels;

namespace NCafe.Barista.Domain.Queries;

public record GetOrders : IQuery<BaristaOrder[]>;

internal sealed class GetOrdersHandler : IQueryHandler<GetOrders, BaristaOrder[]>
{
    private readonly IReadModelRepository<BaristaOrder> orderRepository;

    public GetOrdersHandler(IReadModelRepository<BaristaOrder> orderRepository)
    {
        this.orderRepository = orderRepository;
    }

    public Task<BaristaOrder[]> HandleAsync(GetOrders query)
    {
        var products = orderRepository.GetAll()
            .ToArray();
        return Task.FromResult(products);
    }
}
