using Confluent.Kafka;
using NCafe.Abstractions.Commands;
using NCafe.Abstractions.EventBus.Events;
using NCafe.Barista.Application.Commands;
using System.Text.Json;

namespace NCafe.Barista.Api.EventBus;

public class OrdersConsumer : BackgroundService
{
    private const string Topic = "orders";

    private readonly ConsumerConfig consumerConfig;
    private readonly ICommandDispatcher commandDispatcher;
    private readonly ILogger<OrdersConsumer> logger;

    public OrdersConsumer(
        ICommandDispatcher commandDispatcher,
        IConfiguration configuration,
        ILogger<OrdersConsumer> logger)
    {
        consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["KafkaConfig:BootstrapServers"],
            GroupId = configuration["KafkaConfig:BaristaConsumerGroupName"]
        };
        this.commandDispatcher = commandDispatcher;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<Null, string>(consumerConfig)
            .SetErrorHandler((_, e) => logger.LogError("Consumer error: {reason}", e.Reason))
            .Build();

        consumer.Subscribe(Topic);

        try
        {
            while (true)
            {
                try
                {
                    var cr = consumer.Consume(stoppingToken);
                    logger.LogInformation("Consuming Order {orderPayload}", cr.Message.Value);

                    var orderPlaced = JsonSerializer.Deserialize<OrderPlaced>(cr.Message.Value);

                    await commandDispatcher.DispatchAsync(new PlaceOrder(orderPlaced.Id, orderPlaced.ProductId, orderPlaced.Quantity));
                }
                catch (ConsumeException e)
                {
                    logger.LogError(e, "Consumer error: {reason}", e.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }
    }
}
