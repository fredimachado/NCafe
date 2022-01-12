using EventStore.Client;
using NCafe.Abstractions.ReadModels;
using NCafe.Admin.Domain.Events;
using NCafe.Admin.Domain.ReadModels;
using System.Text.Json;

namespace NCafe.Admin.Api.Projections;

public class ProductProjectionService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly EventStoreClient eventStoreClient;
    private readonly IReadModelRepository<Product> productReadRepository;
    private readonly ILogger logger;

    public ProductProjectionService(
        IServiceProvider serviceProvider,
        EventStoreClient eventStoreClient,
        IReadModelRepository<Product> productReadRepository,
        ILogger<ProductProjectionService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.eventStoreClient = eventStoreClient;
        this.productReadRepository = productReadRepository;
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
            productReadRepository.Add(product);
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
