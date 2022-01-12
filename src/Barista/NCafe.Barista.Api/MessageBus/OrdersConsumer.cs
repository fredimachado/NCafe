using Confluent.Kafka;
using NCafe.Abstractions.Commands;
using NCafe.Abstractions.EventBus.Events;
using NCafe.Barista.Domain.Commands;
using System.Text.Json;

namespace NCafe.Barista.Api.MessageBus;

public class OrdersConsumer : BackgroundService
{
    private const string Topic = "orders";

    private readonly ConsumerConfig consumerConfig;
    private readonly ICommandDispatcher commandDispatcher;
    private readonly ILogger<OrdersConsumer> logger;

    private IConsumer<Null, string> consumer;

    public OrdersConsumer(
        ICommandDispatcher commandDispatcher,
        IConfiguration configuration,
        ILogger<OrdersConsumer> logger)
    {
        consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["KafkaConfig:BootstrapServers"],
            GroupId = configuration["KafkaConfig:BaristaConsumerGroupName"],
            EnableAutoCommit = false,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        this.commandDispatcher = commandDispatcher;
        this.logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting consumer background service.");

        consumer = new ConsumerBuilder<Null, string>(consumerConfig)
                    .SetErrorHandler((_, e) => logger.LogError("Consumer error: {reason}", e.Reason))
                    .Build();

        consumer.Subscribe(Topic);

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    logger.LogInformation("Consuming Order {orderPayload}", result.Message.Value);

                    var orderPlaced = JsonSerializer.Deserialize<OrderPlaced>(result.Message.Value);

                    await commandDispatcher.DispatchAsync(new PlaceOrder(orderPlaced.Id, orderPlaced.ProductId, orderPlaced.Quantity));

                    consumer.Commit(result);
                }
                catch (ConsumeException e)
                {
                    logger.LogError(e, "Consumer error: {reason}", e.Error.Reason);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "There was an error consuming a message.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Closing consumer.");
            consumer.Close();
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping consumer background service.");

        consumer.Unsubscribe();
        consumer.Close();

        return base.StopAsync(cancellationToken);
    }
}
