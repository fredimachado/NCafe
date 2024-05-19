using NCafe.Core.MessageBus;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace NCafe.Infrastructure.MessageBus;

internal class RabbitMqPublisher(IConnection connection, ILogger<RabbitMqPublisher> logger) : IPublisher
{
    private readonly IConnection _connection = connection;
    private readonly ILogger _logger = logger;

    public Task Publish<T>(string queueName, T message) where T : class, IBusMessage
    {
        using var channel = RabbitMqHelper.CreateModelAndDeclareTestQueue(_connection, queueName);

        RabbitMqHelper.AddMessagingTags(Activity.Current, string.Empty, queueName);

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        channel.BasicPublish(exchange: string.Empty,
                             routingKey: queueName,
                             basicProperties: null,
                             body: body);

        _logger.LogInformation("Published {MessageType} Message: {@Message}", message.GetType().Name, message);

        return Task.CompletedTask;
    }
}
