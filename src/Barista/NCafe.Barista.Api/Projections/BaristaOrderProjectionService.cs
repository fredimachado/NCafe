using NCafe.Barista.Domain.Events;
using NCafe.Barista.Domain.ReadModels;
using NCafe.Infrastructure.EventStore;

namespace NCafe.Barista.Api.Projections;

public class BaristaOrderProjectionService : BackgroundService
{
    private readonly EventStoreProjectionService<BaristaOrder> projectionService;

    private const string streamName = "baristaOrder";

    public BaristaOrderProjectionService(EventStoreProjectionService<BaristaOrder> projectionService)
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
        await projectionService.Start(streamName, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
