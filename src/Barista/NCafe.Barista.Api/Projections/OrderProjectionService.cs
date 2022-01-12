using EventStore.Client;
using NCafe.Abstractions.ReadModels;
using NCafe.Barista.Domain.Events;
using NCafe.Barista.Domain.ReadModels;
using System.Text.Json;

namespace NCafe.Barista.Api.Projections;

public class OrderProjectionService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly EventStoreClient eventStoreClient;
    private readonly IReadModelRepository<BaristaOrder> orderRepository;
    private readonly ILogger logger;

    public OrderProjectionService(
        IServiceProvider serviceProvider,
        EventStoreClient eventStoreClient,
        IReadModelRepository<BaristaOrder> orderRepository,
        ILogger<OrderProjectionService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.eventStoreClient = eventStoreClient;
        this.orderRepository = orderRepository;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();

        await eventStoreClient.SubscribeToStreamAsync(
            "$ce-baristaOrder",
            OrderEventAppeared,
            subscriptionDropped: SubscriptionDropped,
            resolveLinkTos: true,
            cancellationToken: stoppingToken);

        logger.LogInformation("Subscribed to EventStore Stream.", DateTimeOffset.Now);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task OrderEventAppeared(StreamSubscription subscription, ResolvedEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Event.EventType == nameof(OrderPlaced))
        {
            var order = JsonSerializer.Deserialize<BaristaOrder>(@event.Event.Data.Span);
            orderRepository.Add(order);
            logger.LogInformation(
                "Added barista order {orderId}, product {productId}, quantity {quantity}",
                order.Id,
                order.ProductId,
                order.Quantity);
        }
        else if (@event.Event.EventType == nameof(OrderPrepared))
        {
            using var document = JsonDocument.Parse(@event.Event.Data);
            var orderId = document.RootElement
                .GetProperty(nameof(BaristaOrder.Id))
                .GetString();

            var order = orderRepository.GetById(Guid.Parse(orderId));
            order.IsCompleted = true;

            orderRepository.Add(order);
        }

        return Task.CompletedTask;
    }

    private void SubscriptionDropped(StreamSubscription subscription, SubscriptionDroppedReason reason, Exception exception)
    {
        logger.LogError("Subscription Dropped.", DateTimeOffset.Now);
    }
}
