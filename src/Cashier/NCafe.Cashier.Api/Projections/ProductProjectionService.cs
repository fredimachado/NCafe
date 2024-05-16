using NCafe.Cashier.Domain.ReadModels;
using NCafe.Core.Projections;

namespace NCafe.Cashier.Api.Projections;

public class ProductProjectionService : BackgroundService
{
    private readonly IProjectionService<Product> projectionService;

    public ProductProjectionService(IProjectionService<Product> projectionService)
    {
        this.projectionService = projectionService;

        projectionService.OnCreate<ProductCreated>(@event => new Product
        {
            Id = @event.Id,
            Name = @event.Name,
            Price = @event.Price
        });
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await projectionService.Start(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
