using NCafe.Admin.Domain.Events;
using NCafe.Admin.Domain.ReadModels;
using NCafe.Core.Projections;

namespace NCafe.Admin.Api.Projections;

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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await projectionService.Start(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
