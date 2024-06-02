using NCafe.Cashier.Domain.ReadModels;
using NCafe.Core.Projections;

namespace NCafe.Cashier.Api.Projections;

public class ProductProjectionService : BackgroundService
{
    private readonly IProjectionService<Product> _projectionService;

    public ProductProjectionService(IProjectionService<Product> projectionService)
    {
        _projectionService = projectionService;

        projectionService.OnCreate<ProductCreated>(@event => new Product
        {
            Id = @event.Id,
            Name = @event.Name,
            Price = @event.Price,
        });

        projectionService.OnUpdate<ProductDeleted>(
            product => product.Id,
            (@event, product) => product.IsDeleted = true);
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _projectionService.Start(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
