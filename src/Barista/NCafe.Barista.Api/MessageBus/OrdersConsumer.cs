using EasyNetQ;
using NCafe.Abstractions.Commands;
using NCafe.Abstractions.MessageBus.Events;
using NCafe.Barista.Domain.Commands;

namespace NCafe.Barista.Api.MessageBus;

public class OrdersConsumer : IHostedService
{
    private const string Queue = "barista_queue";
    private const string Topic = "orders";

    private readonly IBus bus;
    private readonly ICommandDispatcher commandDispatcher;
    private readonly ILogger<OrdersConsumer> logger;

    public OrdersConsumer(
        ICommandDispatcher commandDispatcher,
        IConfiguration configuration,
        ILogger<OrdersConsumer> logger)
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
                config.WithDurable(true);
                config.WithQueueName(Queue);
                config.WithTopic(Topic);
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
