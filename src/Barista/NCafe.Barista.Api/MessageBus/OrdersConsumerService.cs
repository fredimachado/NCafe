using Microsoft.AspNetCore.SignalR;
using NCafe.Barista.Api.Hubs;
using NCafe.Barista.Domain.Commands;
using NCafe.Core.Commands;
using NCafe.Core.MessageBus.Events;
using NCafe.Infrastructure.MessageBus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;

namespace NCafe.Barista.Api.MessageBus;

// TODO: Create abstraction to remove RabbitMq dependency
public class OrdersConsumerService(
    IConnection connection,
    ICommandDispatcher commandDispatcher,
    IHubContext<OrderHub> hubContext,
    ILogger<OrdersConsumerService> logger) : IHostedService
{
    private const string Queue = "orders_queue";

    private readonly IConnection _connection = connection;
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
    private readonly IHubContext<OrderHub> _hubContext = hubContext;
    private readonly ILogger _logger = logger;

    private static readonly ActivitySource ConsumerActivitySource = new("NCafe.MessageBus.Consumer");

    private IModel _channel;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Subscribing to {Queue}.", Queue);

        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: Queue,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += MessageReceived;
        _channel.BasicConsume(queue: Queue,
                             autoAck: false,
                             consumer: consumer);

        return Task.CompletedTask;
    }

    private async void MessageReceived(object sender, BasicDeliverEventArgs e)
    {
        // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md#span-name
        using var activity = ConsumerActivitySource.StartActivity($"{e.RoutingKey} receive", ActivityKind.Consumer);

        var message = JsonSerializer.Deserialize<OrderPlaced>(e.Body.ToArray());

        _logger.LogInformation("Received {MessageType} Message: {@Message}", message.GetType().Name, message);

        activity?.SetTag("message", message);

        // Common tags (RabbitMqHelper)
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination_kind", "queue");
        activity?.SetTag("messaging.destination", string.Empty);
        activity?.SetTag("messaging.rabbitmq.routing_key", Queue);

        // Dispatch domain command
        await _commandDispatcher.DispatchAsync(new PlaceOrder(message.Id, message.ProductId, message.Quantity));

        // Notify clients
        await _hubContext.Clients.All.SendAsync(
            "ReceiveOrder",
            new Shared.Hubs.Order(message.Id, message.ProductId, message.Quantity));

        _channel.BasicAck(e.DeliveryTag, multiple: false);

        _logger.LogInformation("Finished processing message.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        ConsumerActivitySource?.Dispose();
        _channel?.Dispose();
        _connection?.Dispose();

        return Task.CompletedTask;
    }
}
