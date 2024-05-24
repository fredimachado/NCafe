using NCafe.Barista.Domain.Events;
using NCafe.Barista.Domain.ReadModels;
using NCafe.Core.Projections;

namespace NCafe.Barista.Api.Projections;

public class BaristaOrderProjectionService : BackgroundService
{
    private readonly IProjectionService<BaristaOrder> _projectionService;

    public BaristaOrderProjectionService(IProjectionService<BaristaOrder> projectionService)
    {
        _projectionService = projectionService;

        projectionService.OnCreate<OrderPlaced>(@event => new BaristaOrder
        {
            Id = @event.Id,
            CustomerName = @event.Customer,
            Items = @event.OrderItems.Select(item => new BaristaOrderItem
            {
                Name = item.Name,
                Quantity = item.Quantity
            }).ToArray()
        });

        projectionService.OnUpdate<OrderPrepared>(
            order => order.Id,
            (@event, order) => order.IsCompleted = true);
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _projectionService.Start(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
