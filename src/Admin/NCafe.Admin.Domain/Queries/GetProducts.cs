using MediatR;
using NCafe.Admin.Domain.ReadModels;
using NCafe.Core.ReadModels;

namespace NCafe.Admin.Domain.Queries;

public record GetProducts : IRequest<Product[]>;

internal sealed class GetProductsHandler(IReadModelRepository<Product> productRepository) : IRequestHandler<GetProducts, Product[]>
{
    private readonly IReadModelRepository<Product> _productRepository = productRepository;

    public Task<Product[]> Handle(GetProducts query, CancellationToken cancellation)
    {
        var products = _productRepository.GetAll()
            .ToArray();
        return Task.FromResult(products);
    }
}
