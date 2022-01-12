using NCafe.Abstractions.Queries;
using NCafe.Abstractions.ReadModels;
using NCafe.Barista.Domain.ReadModels;

namespace NCafe.Barista.Domain.Queries;

public record GetOrders : IQuery<Orders>;

public sealed class Orders
{
    public IEnumerable<BaristaOrder> Items { get; set; }
}

internal sealed class GetOrdersHandler : IQueryHandler<GetOrders, Orders>
{
    private readonly IReadModelRepository<BaristaOrder> orderRepository;

    public GetOrdersHandler(IReadModelRepository<BaristaOrder> orderRepository)
    {
        this.orderRepository = orderRepository;
    }

    public Task<Orders> HandleAsync(GetOrders query)
    {
        var products = new Orders()
        {
            Items = orderRepository.GetAll()
        };
        return Task.FromResult(products);
    }
}
