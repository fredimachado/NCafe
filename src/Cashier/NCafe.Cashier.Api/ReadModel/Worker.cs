using EventStore.Client;
using NCafe.Abstractions.ReadModels;
using NCafe.Cashier.Application.ReadModels;
using NCafe.Cashier.Application.ReadModels.Events;
using System.Text.Json;

namespace NCafe.Cashier.Api.ReadModel;

public class Worker : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly EventStoreClient eventStoreClient;
    private readonly IReadModelRepository<Product> productRepository;
    private readonly ILogger logger;

    public Worker(
        IServiceProvider serviceProvider,
        EventStoreClient eventStoreClient,
        IReadModelRepository<Product> productRepository,
        ILogger<Worker> logger)
    {
        this.serviceProvider = serviceProvider;
        this.eventStoreClient = eventStoreClient;
        this.productRepository = productRepository;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();

        await eventStoreClient.SubscribeToStreamAsync(
            "$ce-product",
            ProductEventAppeared,
            subscriptionDropped: SubscriptionDropped,
            resolveLinkTos: true,
            cancellationToken: stoppingToken);

        logger.LogInformation("Subscribed to EventStore Stream.", DateTimeOffset.Now);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task ProductEventAppeared(StreamSubscription subscription, ResolvedEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Event.EventType == nameof(ProductCreated))
        {
            var state = JsonSerializer.Deserialize<ProductCreated>(@event.Event.Data.Span);
            var product = new Product()
            {
                Id = state.Id,
                Name = state.Name,
                Price = state.Price
            };
            productRepository.Add(product);
            logger.LogInformation(
                "Added product '{productName}' ({productId}) with price ${productPrice}",
                product.Name,
                product.Id,
                product.Price);
        }

        return Task.CompletedTask;
    }

    private void SubscriptionDropped(StreamSubscription arg1, SubscriptionDroppedReason arg2, Exception arg3)
    {
        logger.LogError("Subscription Dropped.", DateTimeOffset.Now);
    }
}
