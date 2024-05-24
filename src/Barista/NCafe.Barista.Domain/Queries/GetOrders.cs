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
        var products = _orderRepository.GetAll()
            .ToArray();
        return Task.FromResult(products);
    }
}
