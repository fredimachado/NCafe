using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCafe.Core.MessageBus;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text.Json;

namespace NCafe.Infrastructure.MessageBrokers.RabbitMQ;

internal class RabbitMqPublisher : IPublisher
{
    private readonly IConnection _connection;
    private readonly RabbitMqSettings _settings;
    private readonly ILogger _logger;

    public RabbitMqPublisher(IConnection connection, IOptions<RabbitMqSettings> options, ILogger<RabbitMqPublisher> logger)
    {
        _connection = connection;
        _settings = options.Value;
        _logger = logger;
    }

    public Task Publish<T>(T message) where T : class, IBusMessage
    {
        var routingKey = JsonNamingPolicy.SnakeCaseLower.ConvertName(typeof(T).Name);
        var exchange = ExchangeNameProvider.Get(_settings.ExchangeName);

        using var channel = _connection.CreateModel();

        RabbitMqHelper.AddMessagingTags(Activity.Current, message, ExchangeType.Topic, exchange, routingKey);

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        var basicProperties = channel.CreateBasicProperties();
        basicProperties.MessageId = Guid.NewGuid().ToString("N");
        basicProperties.CorrelationId = Guid.NewGuid().ToString("N");
        basicProperties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        channel.BasicPublish(exchange,
                             routingKey,
                             basicProperties,
                             body);

        _logger.LogInformation("Published {MessageType} message with routing key {RoutingKey} to Exchange {Exchange}. Message: {@Message}",
            message.GetType().Name, routingKey, exchange, message);

        return Task.CompletedTask;
    }
}
