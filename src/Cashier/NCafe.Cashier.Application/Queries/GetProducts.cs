using NCafe.Abstractions.Queries;
using NCafe.Abstractions.ReadModels;
using NCafe.Cashier.Application.ReadModels;

namespace NCafe.Cashier.Application.Queries;

public record GetProducts : IQuery<Products>;

public sealed class Products
{
    public IEnumerable<Product> Items { get; set; }
}

internal sealed class GetProductsHandler : IQueryHandler<GetProducts, Products>
{
    private readonly IReadModelRepository<Product> productRepository;

    public GetProductsHandler(IReadModelRepository<Product> productRepository)
    {
        this.productRepository = productRepository;
    }

    public Task<Products> HandleAsync(GetProducts query)
    {
        var products = new Products()
        {
            Items = productRepository.GetAll()
        };
        return Task.FromResult(products);
    }
}
