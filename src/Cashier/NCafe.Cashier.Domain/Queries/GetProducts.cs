using NCafe.Cashier.Domain.ReadModels;
using NCafe.Core.Queries;
using NCafe.Core.ReadModels;

namespace NCafe.Cashier.Domain.Queries;

public record GetProducts : IQuery<Product[]>;

internal sealed class GetProductsHandler(IReadModelRepository<Product> productRepository)
    : IQueryHandler<GetProducts, Product[]>
{
    private readonly IReadModelRepository<Product> _productRepository = productRepository;

    public Task<Product[]> HandleAsync(GetProducts query)
    {
        var products = _productRepository.GetAll()
            .ToArray();
        return Task.FromResult(products);
    }
}
