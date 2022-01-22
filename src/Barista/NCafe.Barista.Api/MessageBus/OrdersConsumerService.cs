using EasyNetQ;
using NCafe.Barista.Domain.Commands;
using NCafe.Core.Commands;
using NCafe.Core.MessageBus.Events;

namespace NCafe.Barista.Api.MessageBus;

public class OrdersConsumerService : IHostedService
{
    private const string Queue = "barista_queue";
    private const string Topic = "orders";

    private readonly IBus bus;
    private readonly ICommandDispatcher commandDispatcher;
    private readonly ILogger logger;

    public OrdersConsumerService(
        ICommandDispatcher commandDispatcher,
        IConfiguration configuration,
        ILogger<OrdersConsumerService> logger)
    {
        bus = RabbitHutch.CreateBus(configuration.GetConnectionString("RabbitMq"));
        this.commandDispatcher = commandDispatcher;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var subscriptionId = Guid.NewGuid().ToString();
        await bus.PubSub.SubscribeAsync<OrderPlaced>(
            subscriptionId,
            MessageReceived,
            config =>
            {
                config.WithDurable(true)
                      .WithQueueName(Queue)
                      .WithTopic(Topic);
            },
            cancellationToken);
    }

    private async Task MessageReceived(OrderPlaced orderPlaced, CancellationToken cancellationToken)
    {
        await commandDispatcher.DispatchAsync(new PlaceOrder(orderPlaced.Id, orderPlaced.ProductId, orderPlaced.Quantity));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        bus?.Dispose();

        return Task.CompletedTask;
    }
}
