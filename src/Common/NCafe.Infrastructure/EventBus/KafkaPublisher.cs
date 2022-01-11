using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NCafe.Abstractions.EventBus;
using System.Text.Json;

namespace NCafe.Infrastructure.EventBus;

public class KafkaPublisher : IPublisher
{
    private readonly IConfiguration configuration;
    private readonly ILogger logger;

    public KafkaPublisher(IConfiguration configuration, ILogger<KafkaPublisher> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task Publish<T>(string topicName, T message) where T : class, IBusMessage
    {
        using var producer =
            new ProducerBuilder<Null, string>(new ProducerConfig()
            {
                BootstrapServers = configuration["KafkaConfig:BootstrapServers"]
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
