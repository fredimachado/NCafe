using NCafe.Cashier.Domain.ReadModels;
using NCafe.Infrastructure.EventStore;

namespace NCafe.Cashier.Api.Projections;

public class ProductProjectionService : BackgroundService
{
    private readonly EventStoreProjectionService<Product> projectionService;

    private const string streamName = "product";

    public ProductProjectionService(EventStoreProjectionService<Product> projectionService)
    {
        this.projectionService = projectionService;

        projectionService.OnCreate<ProductCreated>(@event => new Product
        {
            Id = @event.Id,
            Name = @event.Name,
            Price = @event.Price
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await projectionService.Start(streamName, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
