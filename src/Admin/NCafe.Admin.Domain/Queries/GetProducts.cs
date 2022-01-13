using NCafe.Abstractions.Queries;
using NCafe.Abstractions.ReadModels;
using NCafe.Admin.Domain.ReadModels;

namespace NCafe.Admin.Domain.Queries;

public record GetProducts : IQuery<Product[]>;

internal sealed class GetProductsHandler : IQueryHandler<GetProducts, Product[]>
{
    private readonly IReadModelRepository<Product> productRepository;

    public GetProductsHandler(IReadModelRepository<Product> productRepository)
    {
        this.productRepository = productRepository;
    }

    public Task<Product[]> HandleAsync(GetProducts query)
    {
        var products = productRepository.GetAll()
            .ToArray();
        return Task.FromResult(products);
    }
}
