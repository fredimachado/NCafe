using EasyNetQ;
using Microsoft.AspNetCore.SignalR;
using NCafe.Barista.Api.Hubs;
using NCafe.Barista.Domain.Commands;
using NCafe.Core.Commands;
using NCafe.Core.MessageBus.Events;

namespace NCafe.Barista.Api.MessageBus;

public class OrdersConsumerService(
    ICommandDispatcher commandDispatcher,
    IConfiguration configuration,
    IHubContext<OrderHub> hubContext,
    ILogger<OrdersConsumerService> logger) : IHostedService
{
    private const string Queue = "barista_queue";
    private const string Topic = "orders";

    private readonly IBus _bus = RabbitHutch.CreateBus(configuration.GetConnectionString("RabbitMq"));
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
    private readonly IHubContext<OrderHub> _hubContext = hubContext;
    private readonly ILogger _logger = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var subscriptionId = Guid.NewGuid().ToString();
        await _bus.PubSub.SubscribeAsync<OrderPlaced>(
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
        _logger.LogInformation("Received OrderPlaced Event: {@OrderPlaced}", orderPlaced);

        await _commandDispatcher.DispatchAsync(new PlaceOrder(orderPlaced.Id, orderPlaced.ProductId, orderPlaced.Quantity));

        await _hubContext.Clients.All.SendAsync(
            "ReceiveOrder",
            new Shared.Hubs.Order(orderPlaced.Id, orderPlaced.ProductId, orderPlaced.Quantity),
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _bus?.Dispose();

        return Task.CompletedTask;
    }
}
