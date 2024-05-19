using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCafe.Core.MessageBus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace NCafe.Infrastructure.MessageBrokers.RabbitMQ;
internal class RabbitMqConsumerService(
    IServiceProvider serviceProvider,
    IConnection connection,
    IMessageSubscriber messageSubscriber,
    IOptions<RabbitMqSettings> options,
    ILogger<RabbitMqConsumerService> logger) : IHostedService, IDisposable
{
    private readonly ConcurrentDictionary<string, IModel> _channels = new();
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IConnection _connection = connection;
    private readonly IMessageSubscriber _messageSubscriber = messageSubscriber;
    private readonly RabbitMqSettings _settings = options.Value;
    private readonly ILogger _logger = logger;

    private readonly static ActivitySource ConsumerActivitySource = new("NCafe.MessageBus.Consumer");

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var handler in _messageSubscriber.Handlers)
        {
            try
            {
                Subscribe(handler.Key, handler.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error while subscribing to RabbitMQ.");
            }
        }

        return Task.CompletedTask;
    }

    private void Subscribe(Type type, Func<IServiceProvider, object, Task> handler)
    {
        var exchange = GeAttribute(type)?.ExchangeName ?? ExchangeNameProvider.Get(_settings.ExchangeName);
        var routingKey = JsonNamingPolicy.SnakeCaseLower.ConvertName(type.Name);
        var queue = $"{_settings.QueuePrefix}/{exchange}.{routingKey}";

        var channelKey = GetChannelKey(exchange, queue, routingKey);
        if (_channels.ContainsKey(channelKey))
        {
            return;
        }

        var channel = _connection.CreateModel();
        if (!_channels.TryAdd(channelKey, channel))
        {
            _logger.LogError("Error while adding RabbitMQ channel for exchange '{Exchange}', queue '{Queue}', routingKey '{RoutingKey}'.",
                exchange, queue, routingKey);
            channel.Dispose();
            return;
        }

        _logger.LogInformation("Added the channel: {ChannelNumber} for exchange '{Exchange}', queue '{Queue}', routingKey '{RoutingKey}'.",
            channel.ChannelNumber, exchange, queue, routingKey);

        channel.QueueDeclare(queue);

        channel.QueueBind(queue, exchange, routingKey);
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, args) =>
        {
            using var scope = _serviceProvider.CreateScope();

            // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
            // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md#span-name
            using var activity = ConsumerActivitySource.StartActivity($"{args.RoutingKey} receive", ActivityKind.Consumer);

            try
            {
                var messageId = args.BasicProperties.MessageId;
                var correlationId = args.BasicProperties.CorrelationId;
                var timestamp = args.BasicProperties.Timestamp.UnixTime;
                var message = JsonSerializer.Deserialize(args.Body.Span, type);

                _logger.LogInformation("Received {MessageType} Message: {@Message}", message.GetType().Name, message);

                RabbitMqHelper.AddMessagingTags(activity, message, ExchangeType.Topic, args.Exchange, args.RoutingKey);

                await handler(scope.ServiceProvider, message);

                channel.BasicAck(args.DeliveryTag, false);
                await Task.Yield();

                _logger.LogInformation("Finished processing message.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling message.");

                channel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
                await Task.Yield();
            }
        };

        channel.BasicConsume(queue, autoAck: false, consumer);

        _logger.LogInformation("Subscribed to {MessageType} with routing key {RoutingKey} on exchange {Exchange}.",
            type.Name, routingKey, exchange);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        foreach (var (key, channel) in _channels)
        {
            channel?.Dispose();
            _channels.TryRemove(key, out _);
        }

        try
        {
            _connection?.Dispose();
        }
        catch
        {
        }

        ConsumerActivitySource?.Dispose();
    }

    private static MessageAttribute GeAttribute(MemberInfo type)
        => type.GetCustomAttribute<MessageAttribute>();

    private static string GetChannelKey(string exchange, string queue, string routingKey)
        => $"{exchange}:{queue}:{routingKey}";
}
