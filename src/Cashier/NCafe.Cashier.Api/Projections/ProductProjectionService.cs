using EventStore.Client;
using NCafe.Abstractions.ReadModels;
using NCafe.Cashier.Domain.ReadModels;
using System.Text.Json;

namespace NCafe.Cashier.Api.ReadModel;

public class ProductProjectionService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly EventStoreClient eventStoreClient;
    private readonly IReadModelRepository<Product> productRepository;
    private readonly ILogger logger;

    private const string ProductCreatedEventName = "ProductCreated";

    public ProductProjectionService(
        IServiceProvider serviceProvider,
        EventStoreClient eventStoreClient,
        IReadModelRepository<Product> productRepository,
        ILogger<ProductProjectionService> logger)
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
        if (@event.Event.EventType == ProductCreatedEventName)
        {
            var product = JsonSerializer.Deserialize<Product>(@event.Event.Data.Span);
            productRepository.Add(product);
            logger.LogInformation(
                "Added product '{productName}' ({productId}) with price ${productPrice}",
                product.Name,
                product.Id,
                product.Price);
        }

        return Task.CompletedTask;
    }

    private void SubscriptionDropped(StreamSubscription subscription, SubscriptionDroppedReason reason, Exception exception)
    {
        logger.LogError("Subscription Dropped.", DateTimeOffset.Now);
    }
}
