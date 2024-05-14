using NCafe.Barista.Domain.Events;
using NCafe.Barista.Domain.ReadModels;
using NCafe.Core.Projections;

namespace NCafe.Barista.Api.Projections;

public class BaristaOrderProjectionService : BackgroundService
{
    private readonly IProjectionService<BaristaOrder> projectionService;

    public BaristaOrderProjectionService(IProjectionService<BaristaOrder> projectionService)
    {
        this.projectionService = projectionService;

        projectionService.OnCreate<OrderPlaced>(@event => new BaristaOrder
        {
            Id = @event.Id,
            ProductId = @event.ProductId,
            Quantity = @event.Quantity
        });

        projectionService.OnUpdate<OrderPrepared>(
            order => order.Id,
            (@event, order) => order.IsCompleted = true);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await projectionService.Start(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
