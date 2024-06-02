using MediatR;
using NCafe.Cashier.Domain.ReadModels;
using NCafe.Core.ReadModels;

namespace NCafe.Cashier.Domain.Queries;

public record GetProducts : IRequest<Product[]>;

internal sealed class GetProductsHandler(IReadModelRepository<Product> productRepository)
    : IRequestHandler<GetProducts, Product[]>
{
    private readonly IReadModelRepository<Product> _productRepository = productRepository;

    public Task<Product[]> Handle(GetProducts request, CancellationToken cancellationToken)
    {
        var products = _productRepository.GetAll()
            .Where(p => !p.IsDeleted)
            .ToArray();
        return Task.FromResult(products);
    }
}
