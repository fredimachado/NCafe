using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCafe.Abstractions.EventBus;
using System.Text.Json;

namespace NCafe.Infrastructure.EventBus;

internal class KafkaPublisher : IPublisher
{
    private readonly KafkaOptions kafkaOptions;
    private readonly ILogger logger;

    public KafkaPublisher(IOptions<KafkaOptions> options, ILogger<KafkaPublisher> logger)
    {
        this.kafkaOptions = options.Value;
        this.logger = logger;
    }

    public async Task Publish<T>(string topicName, T message) where T : class, IBusMessage
    {
        using var producer =
            new ProducerBuilder<Null, string>(new ProducerConfig()
            {
                BootstrapServers = kafkaOptions.BootstrapServers
            })
            .Build();

        logger.LogInformation("{producerName} producing on {topicName}.", producer.Name, topicName);

        var json = JsonSerializer.Serialize(message);
        try
        {
            var result = await producer.ProduceAsync(topicName, new Message<Null, string> { Value = json });
            logger.LogInformation("Produced message to: {topicPartitionOffset}", result.TopicPartitionOffset);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error producing message.");
        }
    }
}
