﻿using NCafe.Cashier.Domain.ReadModels;
using NCafe.Core.Queries;
using NCafe.Core.ReadModels;

namespace NCafe.Cashier.Domain.Queries;

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
